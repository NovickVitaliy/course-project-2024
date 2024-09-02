using System.Collections;
using BlazorBootstrap;
using DatingAgencyMS.Client.Features.Meetings.Models.Enum;
using DatingAgencyMS.Client.Features.Meetings.Services;
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
    [Inject] private IMeetingsService MeetingsService { get; init; }
    private ConfirmDialog _dialog = default!;
    private LoggedInUser? _loggedInUser;
    private Modal? _plannedMeetingsByPeriodPromptModal;
    private Modal? _countOfMeetingsBySex;
    private string _sexForCountOfMeeting { get; set; } = string.Empty;
    private string _sexForCountOfMeetingError = string.Empty;
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

    private IEnumerable<(string Month, int MonthIndex)> GetLeftMonthsForYear()
    {
        if (_periodYear != DateTime.Now.Year)
        {
            for (int i = 1; i <= 12; i++)
            {
                yield return (MonthHelper.GetUkrainianMonthName(i), i);
            }

            yield break;
        }
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

    private async Task OnHideCountOfMeetingModal()
    {
        await _countOfMeetingsBySex?.HideAsync();
    }

    private async Task OnCountOfMeetingModalSuccess()
    {
        if (string.IsNullOrEmpty(_sexForCountOfMeeting))
        {
            _sexForCountOfMeetingError = "Статева ознака не може бути пустою";
            return;
        }

        _sexForCountOfMeetingError = string.Empty;
        
        try
        {
            var count = await MeetingsService.GetCountOfConductedMeetingsBySex(_sexForCountOfMeeting, UserState.Value.User.Token);
            await _countOfMeetingsBySex.HideAsync();
            await _dialog.ShowAsync("Кільсть проведених зустрічей", $"Кількість проведених зустрічей для клієнтів з статевою ознакою: '{_sexForCountOfMeeting}'" +
                                                                    $" складає {count} клієнтів.",
                new ConfirmDialogOptions
                {
                    IsVerticallyCentered = true,
                    YesButtonText = "OK",
                    NoButtonColor = ButtonColor.None,
                    NoButtonText = string.Empty
                });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task PromptForSexForCount()
    {
        await _countOfMeetingsBySex?.ShowAsync();
    }
}