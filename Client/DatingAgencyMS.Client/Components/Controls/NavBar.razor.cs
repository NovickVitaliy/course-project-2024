using Blazored.LocalStorage;
using DatingAgencyMS.Client.Constants;
using DatingAgencyMS.Client.Models.Core;
using DatingAgencyMS.Client.Services;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace DatingAgencyMS.Client.Components.Controls;

public partial class NavBar
{
    private LoggedInUser? User { get; set; }
    [Inject] private IDispatcher Dispatcher { get; set; }
    [Inject] private ILocalStorageService LocalStorageService { get; set; }
    [Inject] private IDbAccessService DbAccessService { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }
    protected override void OnInitialized()
    {
        UserState.StateChanged += UserStateChanged;
        base.OnInitialized();
    }

    private void UserStateChanged(object? sender, EventArgs e)
    {
        User = UserState.Value.User;
    }

    private async Task Logout()
    {
        try
        {
            await DbAccessService.Logout(User!.Login, User.Token);
        }
        catch (Exception e)
        {
            //TODO: make some user notification blah blah blah
        }
        finally
        {
            Dispatcher.Dispatch(new SetUserAction(null));
            await LocalStorageService.RemoveItemAsync(UserConstants.UserLocalStorageKey);
            NavigationManager.NavigateTo("/");
        }
    }
}