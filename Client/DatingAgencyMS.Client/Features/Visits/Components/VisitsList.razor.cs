using BlazorBootstrap;
using Common.Filtering.Pagination;
using Common.Filtering.Sorting;
using DatingAgencyMS.Client.Extensions;
using DatingAgencyMS.Client.Features.Visits.Models.Dtos;
using DatingAgencyMS.Client.Features.Visits.Models.Dtos.Requests;
using DatingAgencyMS.Client.Features.Visits.Services;
using DatingAgencyMS.Client.Helpers;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Refit;

namespace DatingAgencyMS.Client.Features.Visits.Components;

public partial class VisitsList : ComponentBase
{
    [Inject] private IState<UserState> UserState { get; init; }
    
    [Inject] private IVisitsService VisitsService { get; init; }
    
    [Inject] private ToastService ToastService { get; init; }
    
    private async Task<GridDataProviderResult<VisitDto>> VisitsDataProvider(GridDataProviderRequest<VisitDto> request)
    {
        try
        {
            var visitsRequest = BuildVisitRequest(request);
            var response = await VisitsService.GetVisits(visitsRequest, UserState.Value.User.Token);

            return new GridDataProviderResult<VisitDto>()
            {
                Data = response.Visits,
                TotalCount = (int?)response.TotalCount
            };
        }
        catch (ApiException e)
        {
            var apiError = e.ToApiError();
            ToastService.Notify(new ToastMessage(ToastType.Danger, apiError.Description));
            return new GridDataProviderResult<VisitDto>();
        }
    }

    private GetVisitsRequest BuildVisitRequest(GridDataProviderRequest<VisitDto> request)
    {
        var idFilter = request.Filters.GetIntegerFilter("Id");
        var clientIdFilter = request.Filters.GetIntegerFilter("ClientId");
        var meetingIdFilter = request.Filters.GetIntegerFilter("MeetingId");
        var visitedFilter = request.Filters.GetBooleanFilter("Visited");
        var sorting = request.Sorting.FirstOrDefault();
        SortingInfo? sortingInfo = null;
        if (sorting is not null)
        {
            sortingInfo = new SortingInfo(sorting.SortString, sorting.GetSortDirection());
        }

        var pagination = new PaginationInfo(request.PageNumber, request.PageSize);
        return new GetVisitsRequest(idFilter, clientIdFilter, meetingIdFilter, visitedFilter, sortingInfo, pagination);
    }
}