using BlazorBootstrap;
using DatingAgencyMS.Client.Extensions;
using DatingAgencyMS.Client.Models.Core;
using DatingAgencyMS.Client.Models.DTOs.User.Requests;
using DatingAgencyMS.Client.Services;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Refit;

namespace DatingAgencyMS.Client.Components.Pages.Users;

public partial class AssignNewRole : ComponentBase
{
    private int? _userId = null;
    private string _userLogin = "";
    private string _errorMessage = "";
    [Inject] private IUsersService UsersService { get; init; }
    [Inject] private IState<UserState> UserState { get; init; }
    [SupplyParameterFromForm] private AssignNewRoleRequest? _assignNewRoleRequest { get; set; } = null;
    [Inject] private ToastService ToastService { get; init; }
    [Inject] private NavigationManager NavigationManager { get; init; }
    private async Task LoadUser()
    {
        if (!string.IsNullOrEmpty(_userLogin))
        {
            _errorMessage = string.Empty;
            try
            {
                var response = await UsersService.GetUser(_userLogin, UserState.Value.User.Token);
                var user = response.User;
                _userId = user.Id;
                _assignNewRoleRequest = new AssignNewRoleRequest
                {
                    Login = user.Login,
                    OldRole = Enum.Parse<DbRoles>(user.Role, true),
                };
            }
            catch (ApiException e)
            {
                ToastService.Notify(new ToastMessage(ToastType.Danger, e.ToApiError().Description));
                _assignNewRoleRequest = null;
            }
        }
        else
        {
            _errorMessage = "Логін не може бути пустим";
        }
    }

    private bool IsOwner()
    {
        return UserState.Value.User!.Role == DbRoles.Owner;
    }

    private async Task OnValidSubmit()
    {
        if (_userId is null || _assignNewRoleRequest is null || UserState.Value.User is null) return;
        try
        {
            await UsersService.AssignNewRoleAsync(_userId.Value, _assignNewRoleRequest, UserState.Value.User.Token);
            ToastService.Notify(new ToastMessage(ToastType.Success,
                "Користувачу було успішно змінено роль. Переводимо вас назад до списку"));
            await Task.Delay(TimeSpan.FromSeconds(2));
            NavigationManager.NavigateTo("/users-management");
        }
        catch (ApiException e)
        {
            var apiError = e.ToApiError();
            ToastService.Notify(new ToastMessage(ToastType.Danger, apiError.Description));
        }
    }
}