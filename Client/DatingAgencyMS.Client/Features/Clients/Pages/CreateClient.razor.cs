using BlazorBootstrap;
using DatingAgencyMS.Client.Extensions;
using DatingAgencyMS.Client.Features.Clients.Models.Dto.Requests;
using DatingAgencyMS.Client.Features.Clients.Services;
using DatingAgencyMS.Client.Models.Core;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Refit;

namespace DatingAgencyMS.Client.Features.Clients.Pages;

public partial class CreateClient
{
    [Inject] public IState<UserState> UserState { get; init; }
    private LoggedInUser? _loggedInUser;
    [Inject] private IClientsService ClientsService { get; init; }
    [Inject] private ToastService ToastService { get; init; }
    private CreateClientRequest _request = new()
    {
        RequestedBy = string.Empty
    };
    private const int MaximumDescriptionLength = 255;

    protected override void OnInitialized()
    {
        _loggedInUser = UserState.Value.User;
        UserState.StateChanged += (_,_) =>
        {
            _loggedInUser = UserState.Value.User;
            _request = new CreateClientRequest
            {
                RequestedBy = _loggedInUser!.Login
            };
        };
        base.OnInitialized();
    }

    protected override void OnParametersSet()
    {
        if (_loggedInUser is not null)
        {
            _request = new CreateClientRequest
            {
                RequestedBy = _loggedInUser.Login
            };   
        }
    }

    private async Task OnValidSubmit()
    {
        if (_loggedInUser is null) return;
        try
        {
            var result = await ClientsService.CreateClient(_request, _loggedInUser.Token);
            if (result.Success)
            {
                ToastService.Notify(new ToastMessage(ToastType.Success, "Клієнт був вдало створений"));
            }
        }
        catch (ApiException e)
        {
            var apiError = e.ToApiError();
            ToastService.Notify(new ToastMessage(ToastType.Danger, apiError.Description));
        }
    }
}