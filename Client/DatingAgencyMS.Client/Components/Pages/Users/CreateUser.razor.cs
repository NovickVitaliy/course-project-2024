using System.Net;
using BlazorBootstrap;
using DatingAgencyMS.Client.Extensions;
using DatingAgencyMS.Client.Models.Core;
using DatingAgencyMS.Client.Models.DTOs.User;
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
    [Inject]
    public IJSRuntime JsRuntime { get; set; }
    
    [Inject]
    public IState<UserState> UserState { get; set; }
    
    [Inject]
    private IUsersService UsersService { get; set; }

    private List<ToastMessage> Toastes { get; set; } = [];
    
    [SupplyParameterFromForm] public CreateUserRequest CreateUserRequest { get; set; } = new();

    public async Task OnValidSubmit()
    {
        try
        {
            CreateUserRequest.RequestedBy = UserState.Value.User!.Login;
            var response = await UsersService.CreateUser(CreateUserRequest, UserState.Value.User!.Token);
            if (response.Code == (int)HttpStatusCode.Created)
            {
                Toastes.Add(new ToastMessage()
                {
                    Type = ToastType.Success,
                    Message = "Користувач був успішно створений"
                });
            }
            else
            {
                Toastes.Add(new ToastMessage()
                {
                    Type = ToastType.Danger,
                    Message = response.Description
                });
            }
        }
        catch (ApiException apiException)
        {
            var error = apiException.ToApiError();
            Toastes.Add(new ToastMessage()
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