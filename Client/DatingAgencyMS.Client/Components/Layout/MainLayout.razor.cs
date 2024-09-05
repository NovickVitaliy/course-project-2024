using System.IdentityModel.Tokens.Jwt;
using Blazored.LocalStorage;
using DatingAgencyMS.Client.Constants;
using DatingAgencyMS.Client.Models.Core;
using DatingAgencyMS.Client.Services;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace DatingAgencyMS.Client.Components.Layout;

public partial class MainLayout
{
    [Inject] private IDispatcher Dispatcher { get; init; }
    [Inject] private ILocalStorageService LocalStorageService { get; init; }
    [Inject] private IDbAccessService DbAccessService { get; init; }
    [Inject] private IState<UserState> UserState { get; init; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadUserFromStorage();
            StateHasChanged();
        }
    }

    private async Task LoadUserFromStorage()
    {
        var user = await LocalStorageService.GetItemAsync<LoggedInUser?>(UserConstants.UserLocalStorageKey);

        if (user is not null && IsJwtTokenValid(user.Token))
        {
            Dispatcher.Dispatch(new SetUserAction(user));
        }
        else
        {
            await LocalStorageService.RemoveItemAsync(UserConstants.UserLocalStorageKey);
        }
    }

    private static bool IsJwtTokenValid(string jwtToken)
    {
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(jwtToken);

        return jwt.ValidTo >= DateTime.UtcNow;
    }
}