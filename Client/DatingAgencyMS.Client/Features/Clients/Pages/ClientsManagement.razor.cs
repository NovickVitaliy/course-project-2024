using BlazorBootstrap;
using DatingAgencyMS.Client.Constants;
using DatingAgencyMS.Client.Extensions;
using DatingAgencyMS.Client.Features.Clients.Services;
using DatingAgencyMS.Client.Models.Core;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Refit;

namespace DatingAgencyMS.Client.Features.Clients.Pages;

public partial class ClientsManagement
{
    private ConfirmDialog _dialog = default!;
    private Modal _byYearQuartersModal = default!;
    private int? _yearForClientsByQuarters;
    private string _yearForClientByQuartersErrorMessage;

    private Modal _registeredByPeriodModal = default!;
    private Constants.RegisteredByPeriod? _period;
    private string _periodErrorMessage;
    [Inject] private NavigationManager NavigationManager { get; init; }
    [Inject] private IState<UserState> UserState { get; init; }
    private LoggedInUser? _loggedInUser;
    [Inject] private IClientsService ClientsService { get; init; }
    [Inject] private ToastService ToastService { get; init; }

    protected override void OnInitialized()
    {
        _loggedInUser = UserState.Value.User;
        UserState.StateChanged += (sender, args) =>
        {
          _loggedInUser = UserState.Value.User;  
        };
        base.OnInitialized();
    }

    private async Task CalculateNumberWhoDeclinedService()
    {
        if (_loggedInUser is null) return;
        try
        {
            var count = await ClientsService.CalculateNumberWhoDeclinedService(_loggedInUser.Token);
            await _dialog.ShowAsync("Клієнти, що відмовились", $"Від послуг сервісу відмовилось {count} клієнтів",
                new ConfirmDialogOptions
                {
                    IsVerticallyCentered = true,
                    YesButtonText = "OK",
                    NoButtonColor = ButtonColor.None,
                    NoButtonText = string.Empty
                });
        }
        catch (ApiException e)
        {
            var apiError = e.ToApiError();
            ToastService.Notify(new ToastMessage(ToastType.Danger, apiError.Description));
        }
    }

    private async Task OnHideModalClick()
    {
        await _byYearQuartersModal.HideAsync();
    }

    private async Task OnShowModalClick()
    {
        await _byYearQuartersModal.ShowAsync();
    }

    private void OnModalSuccessClick()
    {
        if (_yearForClientsByQuarters.HasValue)
        {
            _yearForClientByQuartersErrorMessage = string.Empty;
            NavigationManager.NavigateTo($"/tables/clients/by-year-quarters?year={_yearForClientsByQuarters.Value}");
        }
        else
        {
            _yearForClientByQuartersErrorMessage = "Рік не може бути відсутнім";
        }
    }

    private void RedirectToClientsByPeriod()
    {
        if (_period.HasValue)
        {
            _periodErrorMessage = string.Empty;
            NavigationManager.NavigateTo($"/tables/clients/registered/{_period.ToString()}");
        }
        else
        {
            _periodErrorMessage = "Період часу не може бути відсутнім";
        }
    }

    private async Task ShowRegisteredByPeriodModal()
    {
        await _registeredByPeriodModal.ShowAsync();
    }

    private async Task HideRegisteredByPeriodModal()
    {
        await _registeredByPeriodModal.HideAsync();
    }

    private async Task ShowConfirmDialogToDeleteClientsWhoDeclinedService()
    {
        if (_loggedInUser is null) return;
        try
        {
            var count = await ClientsService.CalculateNumberWhoDeclinedService(_loggedInUser.Token);

            var confirmation = await _dialog.ShowAsync(
                $"Ви справді хочете видалити {count} клієнтів, що відмовились від послуг сервісу?",
                "Ця дія є незворотньою", new ConfirmDialogOptions
                {
                    IsVerticallyCentered = true,
                    YesButtonText = "Видалити",
                    YesButtonColor = ButtonColor.Danger,
                    NoButtonColor = ButtonColor.Secondary,
                    NoButtonText = "Відмінити"
                });

            if (confirmation)
            {
                await ClientsService.DeleteClientsWhoDeclinedService(_loggedInUser.Token);
                ToastService.Notify(new ToastMessage(ToastType.Success,
                    "Клієнти, що відмовились від послуг сервісу було успішно видалено"));
                NavigationManager.Refresh(true);
            }
        }
        catch (ApiException e)
        {
            var apiError = e.ToApiError();
            ToastService.Notify(new ToastMessage(ToastType.Danger, apiError.Description));
        }
    }

    private void ShowClientsWhoDidNotSkipAnyMeeting()
    {
        NavigationManager.NavigateTo("/tables/clients/not-skipped");
    }
}