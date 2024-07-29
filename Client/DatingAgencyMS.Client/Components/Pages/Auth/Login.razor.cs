using BlazorBootstrap;
using Blazored.LocalStorage;
using DatingAgencyMS.Client.Constants;
using DatingAgencyMS.Client.Extensions;
using DatingAgencyMS.Client.Models.DTOs.Auth;
using DatingAgencyMS.Client.Store.UserUseCase;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Refit;

namespace DatingAgencyMS.Client.Components.Pages.Auth;

public partial class Login
{
    [SupplyParameterFromForm] private LoginRequest LoginRequest { get; } = new();

    [Inject] private ILocalStorageService LocalStorageService { get; init; }
    [Inject] private NavigationManager NavigationManager { get; init; }
    [Inject] private ToastService ToastService { get; init; }
    private async Task OnValidSubmit()
    {
        try
        {
            var response = await DbAccessService.Login(LoginRequest);
            await LocalStorageService.SetItemAsync(UserConstants.UserLocalStorageKey, response.ToUser());
            Dispatcher.Dispatch(new SetUserAction(response.ToUser()));
            NavigationManager.NavigateTo("/");
        }
        catch (ApiException e)
        {
            var apiError = e.ToApiError();
            ToastService.Notify(new ToastMessage(ToastType.Danger, apiError.Description));
        }
    }
}