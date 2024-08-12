using BlazorBootstrap;
using Common.Filtering.Pagination;
using Common.Filtering.Sorting;
using DatingAgencyMS.Client.Extensions;
using DatingAgencyMS.Client.Features.Meetings.Models;
using DatingAgencyMS.Client.Features.Meetings.Models.Dtos.Requests;
using DatingAgencyMS.Client.Features.Meetings.Models.Enum;
using DatingAgencyMS.Client.Features.Meetings.Services;
using DatingAgencyMS.Client.Helpers;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Refit;

namespace DatingAgencyMS.Client.Features.Meetings.Components;

public partial class MeetingsList : ComponentBase
{
    [Inject] private IState<UserState> UserState { get; init; }
    [Inject] private ToastService ToastService { get; init; }
    [Inject] private IMeetingsService MeetingsService { get; init; }
    private async Task<GridDataProviderResult<MeetingDto>> MeetingDataProvider(GridDataProviderRequest<MeetingDto> request)
    {
        try
        {
            var meetingsRequest = BuildMeetingsRequest(request);
            var result = await MeetingsService.GetMeetings(meetingsRequest, UserState.Value.User.Token);
            return new GridDataProviderResult<MeetingDto>
            {
                Data = result.Meetings,
                TotalCount = (int?)result.TotalCount
            };
        }
        catch (ApiException e)
        {
            var apiError = e.ToApiError();
            ToastService.Notify(new ToastMessage(ToastType.Danger, apiError.Description));
            return new GridDataProviderResult<MeetingDto> { Data = [], TotalCount = 0};
        }
    }

    private static GetMeetingsRequest BuildMeetingsRequest(GridDataProviderRequest<MeetingDto> request)
    {
        var meetingIdFilter = request.Filters.GetIntegerFilter("MeetingId");
        var dateFilter = request.Filters.GetDateTimeFilter("Date");
        var inviterIdFilter = request.Filters.GetIntegerFilter("InviterId");
        var inviteeIdFilter = request.Filters.GetIntegerFilter("InviteeId");
        var locationFilter = request.Filters.GetStringFilter("Location");
        SortingInfo? sortingInfo = null;
        var sorting = request.Sorting.FirstOrDefault();
        if (sorting is not null)
        {
            sortingInfo = new SortingInfo(sorting.SortString, sorting.GetSortDirection());
        }

        var paginationInfo = new PaginationInfo(request.PageNumber, request.PageSize);
        
        return new GetMeetingsRequest(meetingIdFilter, dateFilter, inviterIdFilter, inviteeIdFilter, locationFilter, sortingInfo, paginationInfo);
    }
}