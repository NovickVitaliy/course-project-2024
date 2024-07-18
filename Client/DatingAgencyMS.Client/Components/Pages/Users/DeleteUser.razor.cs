using BlazorBootstrap;
using DatingAgencyMS.Client.Extensions;
using DatingAgencyMS.Client.Models.DTOs.User.Requests;
using DatingAgencyMS.Client.Services;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Refit;

namespace DatingAgencyMS.Client.Components.Pages.Users;

public partial class DeleteUser : ComponentBase
{
    private readonly DeleteUserRequest _deleteUserRequest = new();

    [Inject] private ToastService ToastService { get; init; }
    [Inject] private IState<UserState> UserState { get; init; }

    [Inject] private IUsersService UsersService { get; init; }

    private async Task OnValidSubmit()
    {
        var user = UserState.Value.User;
        _deleteUserRequest.RequestedBy = user.Login;
        try
        {
            await UsersService.DeleteUser(_deleteUserRequest, user.Token);
            ToastService.Notify(new ToastMessage()
            {
                Type = ToastType.Success,
                Message = "Користувача було видалено"
            });
        }
        catch (ApiException e)
        {
            var apiError = e.ToApiError();
            ToastService.Notify(new ToastMessage()
            {
                Type = ToastType.Danger,
                Message = apiError.Description,
            });
        }
    }
}