using BlazorBootstrap;
using DatingAgencyMS.Client.Extensions;
using DatingAgencyMS.Client.Features.Clients.Models.Dto.Requests;
using DatingAgencyMS.Client.Features.Clients.Services;
using DatingAgencyMS.Client.Models.Core;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Refit;

namespace DatingAgencyMS.Client.Features.Clients.Pages;

public partial class UpdateClient : ComponentBase
{
    [Parameter] public int clientid { get;init; }
    [Inject] private IState<UserState> UserState { get; init; }
    private LoggedInUser? _loggedInUser;
    [Inject] private IClientsService ClientsService { get; init; }
    [Inject] private ToastService ToastService { get; init; }
    [Inject] private NavigationManager NavigationManager { get; init; }
    private UpdateClientRequest _request = new ();
    private const int MaximumDescriptionLength = 255;

    protected override void OnInitialized()
    {
        _loggedInUser = UserState.Value.User;
        UserState.StateChanged += (_, _) => _loggedInUser = UserState.Value.User;
        base.OnInitialized();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (_loggedInUser is not null)
        {
            var client = await ClientsService.GetClientById(clientid, _loggedInUser.Token);
            _request = client.Client.ToUpdateClientRequest();
            _request.RequestedBy = _loggedInUser.Login;
        }
        await base.OnParametersSetAsync();   
    }

    public async Task OnValidSubmit()
    {
        if(_loggedInUser is null) return;
        try
        {
            await ClientsService.UpdateClient(clientid, _request, _loggedInUser.Token);
            ToastService.Notify(new ToastMessage(ToastType.Success, "Дані клієнта були вдало оновлені. Повертаємо вас до списку"));
            await Task.Delay(1000);
            NavigationManager.NavigateTo("/tables/clients");
        }
        catch (ApiException e)
        {
            var apiError = e.ToApiError();
            ToastService.Notify(new ToastMessage(ToastType.Danger, apiError.Description));
        }
    }
}