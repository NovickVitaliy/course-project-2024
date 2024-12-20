using BlazorBootstrap;
using Common.Filtering.Pagination;
using Common.Filtering.Sorting;
using DatingAgencyMS.Client.Extensions;
using DatingAgencyMS.Client.Features.ClientRatings.Models;
using DatingAgencyMS.Client.Features.ClientRatings.Models.Requests;
using DatingAgencyMS.Client.Features.ClientRatings.Services;
using DatingAgencyMS.Client.Helpers;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Refit;

namespace DatingAgencyMS.Client.Features.ClientRatings.Components;

public partial class ClientRatingsList : ComponentBase
{
    [Inject] private IState<UserState> UserState { get; init; }
    [Inject] private IClientRatingsService ClientRatingsService { get; init; }
    [Inject] private ToastService ToastService { get; init; }
    private ConfirmDialog? _confirmDialog = null!;
    private Grid<ClientRatingDto>? _grid = null!;
    
    private async Task<GridDataProviderResult<ClientRatingDto>> ClientRatingsDataProvider(GridDataProviderRequest<ClientRatingDto> request)
    {
        var getRequest = BuildGetRequest(request);
        try
        {
            var response = await ClientRatingsService.GetAsync(getRequest, UserState.Value.User!.Token);
            return new GridDataProviderResult<ClientRatingDto>()
            {
                Data = response.ClientRatings,
                TotalCount = (int?)response.Count
            };
        }
        catch (ApiException e)
        {
            var apiError = e.ToApiError();
            ToastService.Notify(new ToastMessage(ToastType.Danger, apiError.Description));
            return new GridDataProviderResult<ClientRatingDto>()
            {
                Data = [],
                TotalCount = 0
            };
        }
    }
    
    private static GetClientRatingsRequest BuildGetRequest(GridDataProviderRequest<ClientRatingDto> request)
    {
        var idFilter = request.Filters.GetIntegerFilter("Id");
        var clientIdFilter = request.Filters.GetIntegerFilter("ClientId");
        var ratingFilter = request.Filters.GetIntegerFilter("Rating");
        var commentFilter = request.Filters.GetStringFilter("Comment");
        var ratingDateFilter = request.Filters.GetDateTimeFilter("RatingDate");
        SortingInfo? sortingInfo = null;
        var sorting = request.Sorting.FirstOrDefault();
        if (sorting is not null)
        {
            sortingInfo = new SortingInfo(sorting.SortString, sorting.GetSortDirection());
        }

        var paginationInfo = new PaginationInfo(request.PageNumber, request.PageSize);

        return new GetClientRatingsRequest(idFilter, clientIdFilter, ratingFilter, 
            commentFilter, ratingDateFilter, sortingInfo, paginationInfo);
    }
    private async Task ConfirmDeletion(int id)
    {
        if (_confirmDialog is null) return;

        var confirmation = await _confirmDialog.ShowAsync(
            title:$"Ви сравді хочете видалити цей відгук?",
            message1:"Це видалить запис з БД. Ця дія незворотня.",
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
                await ClientRatingsService.DeleteAsync(id, UserState.Value.User!.Token);
                ToastService.Notify(new ToastMessage(ToastType.Success, "Відгук було успішно видалено"));
                await _grid!.RefreshDataAsync();
            }
            catch (ApiException e)
            {
                var apiError = e.ToApiError();
                ToastService.Notify(new (ToastType.Danger, apiError.Description));
            }
        }
    }
}