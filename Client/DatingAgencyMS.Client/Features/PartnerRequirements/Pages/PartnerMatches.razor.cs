using BlazorBootstrap;
using DatingAgencyMS.Client.Extensions;
using DatingAgencyMS.Client.Features.Clients.Services;
using DatingAgencyMS.Client.Models.Core;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Refit;

namespace DatingAgencyMS.Client.Features.PartnerRequirements.Pages;

public partial class PartnerMatches
{
    [Parameter] public int RequirementId { get; init; }
    [SupplyParameterFromQuery] private int ClientId { get; init; }
    [Inject] private IClientsService ClientsService { get; init; }
    [Inject] private IState<UserState> UserState { get; init; }
    private LoggedInUser? _loggedInUser;
    [Inject] private ToastService ToastService { get; init; }
    private ClientDto? _client;

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
            _client = (await ClientsService.GetClientById(ClientId, _loggedInUser.Token)).Client;
        }
        await base.OnParametersSetAsync();
    }

    private async Task<GridDataProviderResult<ClientDto>> ClientDataProvider(GridDataProviderRequest<ClientDto> request)
    {
        if (_loggedInUser is null) return new();
        try
        {
            var result = await ClientsService.GetMatchingClients(ClientId, RequirementId,
                request.PageNumber, request.PageSize, _loggedInUser.Token);
            
            return new GridDataProviderResult<ClientDto>
            {
                Data = result.Clients,
                TotalCount = (int?)result.TotalCount
            };
        }
        catch (ApiException e)
        {
            var apiError = e.ToApiError();
            ToastService.Notify(new ToastMessage(ToastType.Danger, apiError.Description));
            return new GridDataProviderResult<ClientDto>() { };
        }
    }
}