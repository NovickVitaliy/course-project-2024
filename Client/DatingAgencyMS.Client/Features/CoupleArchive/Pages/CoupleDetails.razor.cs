using BlazorBootstrap;
using DatingAgencyMS.Client.Extensions;
using DatingAgencyMS.Client.Features.Clients.Services;
using DatingAgencyMS.Client.Models.Core;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Refit;

namespace DatingAgencyMS.Client.Features.CoupleArchive.Pages;

public partial class CoupleDetails
{
    [Parameter] public int CoupleArchiveId { get; init; }
    [SupplyParameterFromQuery] public int FirstClientId { get; init; }
    [SupplyParameterFromQuery] public int SecondClientId { get; init; }
    [Inject] private ToastService ToastService { get; init; }
    [Inject] private IClientsService ClientsService { get; init; }
    [Inject] private IState<UserState> UserState { get; init; }
    private LoggedInUser? _loggedInUser;
    
    private ClientDto? _firstClient = null;
    private ClientDto? _secondClient = null;

    protected override void OnInitialized()
    {
        _loggedInUser = UserState.Value.User;
        UserState.StateChanged += (_, _) => _loggedInUser = UserState.Value.User;
        base.OnInitialized();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (_loggedInUser is null) return;
        try
        {
            _firstClient = (await ClientsService.GetClientById(FirstClientId, _loggedInUser.Token)).Client;
            _secondClient = (await ClientsService.GetClientById(SecondClientId, _loggedInUser.Token)).Client;
        }
        catch (ApiException e)
        {
            var apiError = e.ToApiError();
            ToastService.Notify(new ToastMessage(ToastType.Danger, apiError.Description));
        }   
        await base.OnParametersSetAsync();
    }
}