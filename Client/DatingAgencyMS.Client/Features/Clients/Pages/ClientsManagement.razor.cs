using BlazorBootstrap;
using DatingAgencyMS.Client.Extensions;
using DatingAgencyMS.Client.Features.Clients.Services;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Refit;

namespace DatingAgencyMS.Client.Features.Clients.Pages;

public partial class ClientsManagement : ComponentBase
{
    private ConfirmDialog _dialog = default!;

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
}