using System.Collections;
using BlazorBootstrap;
using DatingAgencyMS.Client.Features.Meetings.Models.Enum;
using DatingAgencyMS.Client.Helpers;
using DatingAgencyMS.Client.Models.Core;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace DatingAgencyMS.Client.Features.Meetings.Pages;

public partial class MeetingsManagement
{
    [Inject] private NavigationManager NavigationManager { get; init; }
    [Inject] private IState<UserState> UserState { get; init; }
    private LoggedInUser? _loggedInUser;
    private Modal? _plannedMeetingsByPeriodPromptModal;
    private PlannedMeetingByPeriodOption? _periodOption = null;
    private string _periodError = string.Empty;
    private int _periodYear = DateTime.Now.Year;
    private int _periodMonth;
    protected override void OnInitialized()
    {
        _loggedInUser = UserState.Value.User;
        UserState.StateChanged += (_, _) => _loggedInUser = UserState.Value.User;
        base.OnInitialized();
    }

    private async Task PromptForPeriodOfTime()
    {
        await _plannedMeetingsByPeriodPromptModal?.ShowAsync();
    }

    private IEnumerable<(string Month, int MonthIndex)> GetLeftMonths()
    {
        const int amountOfMonths = 12;
        var currentMonthNumber = DateTime.Now.Month;

        while (currentMonthNumber != amountOfMonths)
        {
            currentMonthNumber++;
            yield return (MonthHelper.GetUkrainianMonthName(currentMonthNumber), currentMonthNumber);
        }
    }

    private async Task OnHidePlannedMeetingPromptModalClick()
    {
        await _plannedMeetingsByPeriodPromptModal?.HideAsync();
    }

    private void OnPlannedMeetingPromptModalSuccessClick()
    {
        switch (_periodOption)
        {
            case PlannedMeetingByPeriodOption.CurrentMonth:
                _periodError = string.Empty;
                NavigationManager.NavigateTo($"/tables/meetings/planned?year={DateTime.Now.Year}&month={DateTime.Now.Month}");
                break;
            case PlannedMeetingByPeriodOption.NextMonths:
                _periodError = string.Empty;
                NavigationManager.NavigateTo($"/tables/meetings/planned?year={_periodYear}&month={_periodMonth}");
                break;
            case null:
                _periodError = "Будь ласка виберіть період";
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}