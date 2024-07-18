using System.Net;
using BlazorBootstrap;
using DatingAgencyMS.Client.Extensions;
using DatingAgencyMS.Client.Models.Core;
using DatingAgencyMS.Client.Models.DTOs.User.Requests;
using DatingAgencyMS.Client.Services;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Refit;

namespace DatingAgencyMS.Client.Components.Pages.Users;

public partial class CreateUser : FluxorComponent
{
    [Inject] public IJSRuntime JsRuntime { get; init; }

    [Inject] public IState<UserState> UserState { get; init; }

    [Inject] private IUsersService UsersService { get; init; }

    [Inject] private ToastService ToastService { get; init; }

    [SupplyParameterFromForm] public CreateUserRequest CreateUserRequest { get; set; } = new();

    public async Task OnValidSubmit()
    {
        try
        {
            CreateUserRequest.RequestedBy = UserState.Value.User!.Login;
            var response = await UsersService.CreateUser(CreateUserRequest, UserState.Value.User!.Token);
            if (response.Code == (int)HttpStatusCode.Created)
            {
                ToastService.Notify(new ToastMessage()
                {
                    Type = ToastType.Success,
                    Message = "Користувач був успішно створений"
                });
            }
            else
            {
                ToastService.Notify(new ToastMessage()
                {
                    Type = ToastType.Danger,
                    Message = response.Description
                });
            }
        }
        catch (ApiException apiException)
        {
            var error = apiException.ToApiError();
            ToastService.Notify(new ToastMessage()
            {
                Type = ToastType.Danger,
                Message = error.Description
            });
        }
    }

    private bool IsOwner()
    {
        return UserState.Value.User!.Role == DbRoles.Owner;
    }
}