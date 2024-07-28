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

public partial class CreateClient : ComponentBase
{
    [Inject] public IState<UserState> UserState { get; init; }

    [Inject] private IClientsService ClientsService { get; init; }

    [Inject] private ToastService ToastService { get; init; }
    private CreateClientRequest _request;
    private const int MaximumDescriptionLength = 255;

    protected override void OnParametersSet()
    {
        _request = new CreateClientRequest
        {
            RequestedBy = UserState.Value.User!.Login
        };
    }

    private async Task OnValidSubmit()
    {
        try
        {
            var result = await ClientsService.CreateClient(_request, UserState.Value.User!.Token);
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