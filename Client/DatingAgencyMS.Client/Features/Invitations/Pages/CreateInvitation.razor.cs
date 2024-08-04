using BlazorBootstrap;
using DatingAgencyMS.Client.Extensions;
using DatingAgencyMS.Client.Features.Invitations.Models.Dtos.Requests;
using DatingAgencyMS.Client.Features.Invitations.Services;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Refit;

namespace DatingAgencyMS.Client.Features.Invitations.Pages;

public partial class CreateInvitation : ComponentBase
{
    [Inject] private IState<UserState> UserState { get; init; }
    [Inject] private IInvitationsService InvitationsService { get; init; }
    [Inject] private ToastService ToastService { get; init; }
    [Inject] private NavigationManager NavigationManager { get; init; }
    private CreateInvitationRequest? _request;
    protected override Task OnParametersSetAsync()
    {
        _request = new CreateInvitationRequest()
        {
            RequestedBy = UserState.Value.User.Login
        };
        return base.OnParametersSetAsync();
    }

    private async Task OnValidSubmit(EditContext obj)
    {
        if (_request is null) return;
        try
        {
            await InvitationsService.CreateInvitation(_request, UserState.Value.User.Token);
            ToastService.Notify(new ToastMessage(ToastType.Success, "Запрошення було успішно створено. Перевожу назад до списку запрошень"));
            await Task.Delay(2000);
            NavigationManager.NavigateTo("/tables/invitations");
        }
        catch (ApiException e)
        {
            var apiError = e.ToApiError();
            ToastService.Notify(new ToastMessage(ToastType.Danger, apiError.Description));
        }
    }
}