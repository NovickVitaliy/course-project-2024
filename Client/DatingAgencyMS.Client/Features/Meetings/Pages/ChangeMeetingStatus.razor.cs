using BlazorBootstrap;
using DatingAgencyMS.Client.Extensions;
using DatingAgencyMS.Client.Features.Meetings.Models.Dtos.Requests;
using DatingAgencyMS.Client.Features.Meetings.Models.Enum;
using DatingAgencyMS.Client.Features.Meetings.Services;
using DatingAgencyMS.Client.Models.Core;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Refit;

namespace DatingAgencyMS.Client.Features.Meetings.Pages;

public partial class ChangeMeetingStatus
{
    [Parameter] public int MeetingId { get; init; }
    private ChangeMeetingStatusRequest? _request;
    private bool InviterVisited { get; set; }
    private bool InviteeVisited { get; set; }
    [Inject] private IState<UserState> UserState { get; init; }
    private LoggedInUser? _loggedInUser;
    [Inject] private ToastService ToastService { get; init; }
    [Inject] private IMeetingsService MeetingsService { get; init; }
    
    protected override void OnParametersSet()
    {
        _loggedInUser = UserState.Value.User;
        UserState.StateChanged += (_, _) => _loggedInUser = UserState.Value.User;
        _request = new ChangeMeetingStatusRequest
        {
            MeetingId = MeetingId
        };
        base.OnParametersSet();
    }

    private string GetDetailsAboutStatus()
    {
        return _request.MeetingStatus switch
        {
            MeetingStatus.Match => "Зустрічі буде призначений статус, що партнери підійшли одне одному. Для обох партнерів зустріч буде збережена як відвідана.",
            MeetingStatus.NoMatch => "Зустрічі буде призначений статус, що партнери не підійшли одне одному. Вкажіть хто з парнерів був присутнім та відсутнім.",
            MeetingStatus.Canceled => "Зустрічі буде призначений статус скасовано. Для обох партнерів збережеться запис, що вони не прийшли.",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private async Task OnValidSubmit(EditContext obj)
    {
        if (_request is null || _loggedInUser is null) return;
        ReevaluateRequest();
        try
        {
            await MeetingsService.ChangeMeetingStatus(MeetingId, _request, _loggedInUser.Token);
            ToastService.Notify(new ToastMessage(ToastType.Success, "Статус зустрічі був успішно змінений та необхідні дані було створено та збережено."));
        }
        catch (ApiException e)
        {
            var apiError = e.ToApiError();
            ToastService.Notify(new ToastMessage(ToastType.Danger, apiError.Description));
        }
    }

    private void ReevaluateRequest()
    {
        if (_request.MeetingStatus is MeetingStatus.NoMatch or MeetingStatus.Canceled)
        {
            _request.InviterVisited = InviterVisited;
            _request.InviteeVisited = InviteeVisited;
        }
        else
        {
            _request.InviterVisited = true;
            _request.InviteeVisited = true;
        }
    }
}