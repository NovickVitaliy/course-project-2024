using BlazorBootstrap;
using Common.Filtering.Pagination;
using Common.Filtering.Sorting;
using DatingAgencyMS.Client.Extensions;
using DatingAgencyMS.Client.Features.CoupleArchive.Models.DTOs;
using DatingAgencyMS.Client.Features.CoupleArchive.Models.DTOs.Requests;
using DatingAgencyMS.Client.Features.CoupleArchive.Services;
using DatingAgencyMS.Client.Helpers;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Refit;

namespace DatingAgencyMS.Client.Features.CoupleArchive.Components;

public partial class CoupleArchiveList : ComponentBase
{
    [Inject] private ToastService ToastService { get; init; }
    [Inject] private ICoupleArchiveService CoupleArchiveService { get; init; }
    [Inject] private IState<UserState> UserState { get; init; }
    private long? _countOfThoseWhoSolved = null;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            _countOfThoseWhoSolved = await CoupleArchiveService.GetArchivedCoupleCount(UserState.Value.User.Token);
        }
        catch (ApiException e)
        {
            var apiError = e.ToApiError();
            ToastService.Notify(new ToastMessage(ToastType.Danger, apiError.Description));
        }
    }

    private async Task<GridDataProviderResult<ArchivedCoupleDto>> ArchivedCoupleDataProvider(GridDataProviderRequest<ArchivedCoupleDto> request)
    {
        try
        {
            var archivedCoupleRequest = BuildArchivedCoupleRequest(request);
            var response =
                await CoupleArchiveService.GetArchivedCouples(archivedCoupleRequest, UserState.Value.User.Token);

            return new GridDataProviderResult<ArchivedCoupleDto>()
            {
                Data = response.ArchivedCouples,
                TotalCount = (int?)response.TotalCount
            };
        }
        catch (ApiException e)
        {
            var apiError = e.ToApiError();
            ToastService.Notify(new ToastMessage(ToastType.Danger, apiError.Description));
            return new GridDataProviderResult<ArchivedCoupleDto>();
        }
    }

    private GetArchivedCoupleRequest BuildArchivedCoupleRequest(GridDataProviderRequest<ArchivedCoupleDto> request)
    {
        var coupleArchiveIdFilter = request.Filters.GetIntegerFilter("CoupleArchiveId");
        var firstClientIdFilter = request.Filters.GetIntegerFilter("FirstClientId");
        var secondClientIdFilter = request.Filters.GetIntegerFilter("SecondClientId");
        var coupleCreatedOnFilter = request.Filters.GetDateOnlyFilter("CoupleCreatedOn");
        var additionalInfoFilter = request.Filters.GetStringFilter("AdditionalInfo");
        var archivedOnFilter = request.Filters.GetDateTimeFilter("ArchivedIn");
        SortingInfo? sortingInfo = null;
        var sorting = request.Sorting.FirstOrDefault();
        if (sorting is not null)
        {
            sortingInfo = new SortingInfo(sorting.SortString, sorting.GetSortDirection());
        }

        var paginationInfo = new PaginationInfo(request.PageNumber, request.PageSize);

        return new GetArchivedCoupleRequest(coupleArchiveIdFilter, firstClientIdFilter, secondClientIdFilter,
            coupleCreatedOnFilter, additionalInfoFilter, archivedOnFilter, sortingInfo, paginationInfo);
    }
}