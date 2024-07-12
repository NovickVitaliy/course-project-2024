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
    [SupplyParameterFromForm] private LoginRequest LoginRequest { get; set; } = new();

    [Inject] private ILocalStorageService LocalStorageService { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }
    private async Task OnValidSubmit()
    {
        try
        {
            await JsRuntime.InvokeVoidAsync("console.log", LoginRequest.ToString());
            var response = await DbAccessService.Login(LoginRequest);
            await JsRuntime.InvokeVoidAsync("console.log", response);
            await LocalStorageService.SetItemAsync(UserConstants.UserLocalStorageKey, response.ToUser());
            Dispatcher.Dispatch(new SetUserAction(response.ToUser()));
            NavigationManager.NavigateTo("/");
        }
        catch (ApiException e)
        {
            var apiError = e.ToApiError();
            await JsRuntime.InvokeVoidAsync("console.log", apiError);
            Console.WriteLine(e);
        }
    }
}