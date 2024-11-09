using BlazorBootstrap;
using Common.Filtering.Pagination;
using Common.Filtering.Sorting;
using DatingAgencyMS.Client.Extensions;
using DatingAgencyMS.Client.Features.Complaints.Models;
using DatingAgencyMS.Client.Features.Complaints.Models.Requests;
using DatingAgencyMS.Client.Features.Complaints.Services;
using DatingAgencyMS.Client.Helpers;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Refit;

namespace DatingAgencyMS.Client.Features.Complaints.Components;

public partial class ComplaintsList : ComponentBase
{
    [Inject] private IState<UserState> UserState { get; init; } = null!;
    [Inject] private IComplaintsService ComplaintsService { get; init; } = null!;
    [Inject] private ToastService ToastService { get; init; } = null!;
    private ConfirmDialog? _confirmDialog = null!;
    private Grid<ComplaintDto>? _grid = null!;

    private async Task<GridDataProviderResult<ComplaintDto>> ComplaintsDataProvider(GridDataProviderRequest<ComplaintDto> request)
    {
        var getRequest = BuildGetRequest(request);

        try
        {
            var response = await ComplaintsService.GetAsync(getRequest, UserState.Value.User!.Token);
            return new GridDataProviderResult<ComplaintDto>()
            {
                Data = response.Complaints,
                TotalCount = (int?)response.Count
            };
        }
        catch (ApiException e)
        {
            var apiError = e.ToApiError();
            ToastService.Notify(new ToastMessage());
            return new GridDataProviderResult<ComplaintDto>()
            {
                Data = [],
                TotalCount = 0
            };
        }
    }
    private GetComplaintsRequest BuildGetRequest(GridDataProviderRequest<ComplaintDto> request)
    {
        var complaintIdFilter = request.Filters.GetIntegerFilter("ComplaintId");
        var complainantIdFilter = request.Filters.GetIntegerFilter("ComplainantId");
        var complaineeIdFilter = request.Filters.GetIntegerFilter("ComplaineeId");
        var dateFilter = request.Filters.GetDateTimeFilter("Date");
        var textFilter = request.Filters.GetStringFilter("Text");
        var complaintStatusFilter = request.Filters.GetStringFilter("ComplaintStatus");
        SortingInfo? sortingInfo = null;
        var sorting = request.Sorting.FirstOrDefault();
        if (sorting is not null)
        {
            sortingInfo = new SortingInfo(sorting.SortString, sorting.GetSortDirection());
        }

        var paginationInfo = new PaginationInfo(request.PageNumber, request.PageSize);

        return new GetComplaintsRequest(complaintIdFilter, complainantIdFilter, complaineeIdFilter, dateFilter, textFilter, complaintStatusFilter, sortingInfo, paginationInfo);
    }

    private async Task ConfirmDeletion(int id)
    {
        if (_confirmDialog is null) return;
        
        var confirmation = await _confirmDialog.ShowAsync(
            "Пітвердження видалення",
            "Ви справді хочете видалити запис об'єкту скарги з бази даних?",
            confirmDialogOptions: new ConfirmDialogOptions()
            {
                YesButtonText = "Видалити",
                YesButtonColor = ButtonColor.Danger,
                NoButtonText = "Назад",
                NoButtonColor = ButtonColor.Secondary
            });

        if (confirmation)
        {
            try
            {
                await ComplaintsService.DeleteAsync(id, UserState.Value.User!.Token);
                ToastService.Notify(new ToastMessage(ToastType.Success, "Запис скарги був успішно видалений."));
                await _grid!.RefreshDataAsync();
            }
            catch (ApiException e)
            {
                var apiError = e.ToApiError();
                ToastService.Notify(new ToastMessage(ToastType.Danger, apiError.Description));
            }
        }
    }
}