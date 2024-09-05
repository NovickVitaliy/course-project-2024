using BlazorBootstrap;
using Common.Filtering.Pagination;
using Common.Filtering.Sorting;
using DatingAgencyMS.Client.Extensions;
using DatingAgencyMS.Client.Features.AdditionalContacts.Models;
using DatingAgencyMS.Client.Features.AdditionalContacts.Models.DTOs.Requests;
using DatingAgencyMS.Client.Features.AdditionalContacts.Services;
using DatingAgencyMS.Client.Helpers;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Refit;

namespace DatingAgencyMS.Client.Features.AdditionalContacts.Components;

public partial class AdditionalContactsList : ComponentBase
{
    [Inject] private ToastService ToastService { get; init; }
    [Inject] private IAdditionalContactsService AdditionalContactsService { get; init; }
    [Inject] private IState<UserState> UserState { get; init; }

    private async Task<GridDataProviderResult<AdditionalContactDto>> AdditionalContactsDataProvider(
        GridDataProviderRequest<AdditionalContactDto> request)
    {
        var additionalContactsRequest = BuildRequst(request);
        try
        {
            var response = await AdditionalContactsService.GetAsync(additionalContactsRequest, UserState.Value.User.Token);
            return new GridDataProviderResult<AdditionalContactDto>()
            {
                Data = response.AdditionalContacts,
                TotalCount = (int?)response.TotalCount
            };
        }
        catch (ApiException e)
        {
            var apiError = e.ToApiError();
            ToastService.Notify(new ToastMessage(ToastType.Danger, apiError.Description));
            return new GridDataProviderResult<AdditionalContactDto>() { Data = [], TotalCount = 0 };
        }
    }

    private GetAdditionalContactsRequest BuildRequst<TItem>(GridDataProviderRequest<TItem> request)
    {
        var idFilter = request.Filters.GetIntegerFilter("Id");
        var clientIdFilter = request.Filters.GetIntegerFilter("ClientId");
        var telegramFilter = request.Filters.GetStringFilter("Telegram");
        var facebookFilter = request.Filters.GetStringFilter("Facebook");
        var instagramFilter = request.Filters.GetStringFilter("Instagram");
        var tiktokFilter = request.Filters.GetStringFilter("TikTok");
        SortingInfo? sortingInfo = null;
        var sorting = request.Sorting.FirstOrDefault();
        if (sorting is not null)
        {
            sortingInfo = new SortingInfo(sorting.SortString, sorting.GetSortDirection());
        }

        var paginationInfo = new PaginationInfo(request.PageNumber, request.PageSize);

        return new GetAdditionalContactsRequest(idFilter, clientIdFilter, telegramFilter, facebookFilter,
            instagramFilter, tiktokFilter, sortingInfo, paginationInfo);
    }
}