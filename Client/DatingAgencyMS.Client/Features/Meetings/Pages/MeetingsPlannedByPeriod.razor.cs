using BlazorBootstrap;
using DatingAgencyMS.Client.Extensions;
using DatingAgencyMS.Client.Features.Meetings.Models;
using DatingAgencyMS.Client.Features.Meetings.Models.Dtos.Requests;
using DatingAgencyMS.Client.Features.Meetings.Services;
using DatingAgencyMS.Client.Models.Core;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Refit;

namespace DatingAgencyMS.Client.Features.Meetings.Pages;

public partial class MeetingsPlannedByPeriod
{
    [SupplyParameterFromQuery] private int Year { get; init; }
    [SupplyParameterFromQuery] private int Month { get; init; }
    [Inject] private IState<UserState> UserState { get; init; }
    private LoggedInUser? _loggedInUser;
    [Inject] private IMeetingsService MeetingsService { get; init; }
    [Inject] private ToastService ToastService { get; init; }
    
    protected override void OnInitialized()
    {
        _loggedInUser = UserState.Value.User;
        UserState.StateChanged += (_, _) => _loggedInUser = UserState.Value.User;
        base.OnInitialized();
    }

    private async Task<GridDataProviderResult<MeetingDto>> PlannedMeetingsDataProvider(GridDataProviderRequest<MeetingDto> request)
    {
        try
        {
            var meetingsRequest = new GetPlannedMeetingsForPeriodRequest(Year, Month, request.PageNumber, request.PageSize);
            var result = await MeetingsService.GetPlannedMeetingsForPeriod(meetingsRequest, _loggedInUser.Token);
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
            return new GridDataProviderResult<MeetingDto>(){Data = [],TotalCount = 0};
        }
    }
}