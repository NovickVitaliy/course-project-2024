using BlazorBootstrap;
using DatingAgencyMS.Client.Constants;
using DatingAgencyMS.Client.Extensions;
using DatingAgencyMS.Client.Features.Clients.Services;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Refit;

namespace DatingAgencyMS.Client.Features.Clients.Pages;

public partial class ClientsManagement : FluxorComponent
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

    [Inject] private IClientsService ClientsService { get; init; }
    
    
    private async Task CalculateNumberWhoDeclinedService()
    {
        try
        {
            var count = await ClientsService.CalculateNumberWhoDeclinedService(UserState.Value.User.Token);
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
}