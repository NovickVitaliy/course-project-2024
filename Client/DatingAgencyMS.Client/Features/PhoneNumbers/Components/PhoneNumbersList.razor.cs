using BlazorBootstrap;
using Common.Filtering.Pagination;
using Common.Filtering.Sorting;
using DatingAgencyMS.Client.Extensions;
using DatingAgencyMS.Client.Features.AdditionalContacts.Models;
using DatingAgencyMS.Client.Features.PhoneNumbers.Models;
using DatingAgencyMS.Client.Features.PhoneNumbers.Models.Requests;
using DatingAgencyMS.Client.Features.PhoneNumbers.Services;
using DatingAgencyMS.Client.Helpers;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Refit;

namespace DatingAgencyMS.Client.Features.PhoneNumbers.Components;

public partial class PhoneNumbersList : ComponentBase
{
    private Grid<PhoneNumberDto>? _grid;
    [Inject] private ToastService ToastService { get; init; } = null!;
    [Inject] private IState<UserState> UserState { get; init; } = null!;
    [Inject] private IPhoneNumbersService PhoneNumbersService { get; init; } = null!;
    private ConfirmDialog? _confirmDialog = null!;
    private async Task<GridDataProviderResult<PhoneNumberDto>> PhoneNumbersDataProvider(GridDataProviderRequest<PhoneNumberDto> request)
    {
        var getRequest = BuildRequest(request);

        var response = await PhoneNumbersService.GetAsync(getRequest, UserState.Value.User!.Token);

        return new GridDataProviderResult<PhoneNumberDto>()
        {
            Data = response.PhoneNumbers.ToList(),
            TotalCount = (int?)response.Count
        };
    }
    private GetPhoneNumbersRequest BuildRequest(GridDataProviderRequest<PhoneNumberDto> request)
    {
        var idFilter = request.Filters.GetIntegerFilter("Id");
        var phoneNumberFilter = request.Filters.GetStringFilter("PhoneNumber");
        var additionalContactsIdFilter = request.Filters.GetIntegerFilter("AdditionalContactsId");
        SortingInfo? sortingInfo = null;
        var sorting = request.Sorting.FirstOrDefault();
        if (sorting is not null)
        {
            sortingInfo = new SortingInfo(sorting.SortString, sorting.GetSortDirection());
        }

        var paginationInfo = new PaginationInfo(request.PageNumber, request.PageSize);

        return new GetPhoneNumbersRequest(idFilter, phoneNumberFilter, additionalContactsIdFilter, sortingInfo, paginationInfo);
    }

    private async Task ConfirmDelete(int id)
    {
        var confirmation = await _confirmDialog.ShowAsync("Підтвердження видалення",
            $"Ви справді хочете видалити номер телефону з ID: {id}? ", 
            new ConfirmDialogOptions
            {
                YesButtonText = "ОК",
                YesButtonColor = ButtonColor.Success,
                NoButtonText = "Відмінити",
                NoButtonColor = ButtonColor.Secondary
            });

        if (confirmation)
        {
            try
            {
                await PhoneNumbersService.DeleteAsync(id, UserState.Value.User!.Token);
                await _grid!.RefreshDataAsync();
                ToastService.Notify(new ToastMessage(ToastType.Success, "Номер телефону було успішно видалено."));
            }
            catch (ApiException e)
            {
                var apiError = e.ToApiError();
                ToastService.Notify(new ToastMessage(ToastType.Danger, apiError.Description));
            }
        }
    }
}