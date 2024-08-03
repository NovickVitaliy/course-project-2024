using BlazorBootstrap;
using DatingAgencyMS.Client.Extensions;
using DatingAgencyMS.Client.Features.Clients.Services;
using DatingAgencyMS.Client.Models.Core;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Refit;

namespace DatingAgencyMS.Client.Features.PartnerRequirements.Pages;

public partial class PartnerMatches : ComponentBase
{
    [Parameter] public int RequirementId { get; init; }
    [SupplyParameterFromQuery] private int ClientId { get; init; }
    [Inject] private IClientsService ClientsService { get; init; }
    [Inject] private IState<UserState> UserState { get; init; }
    [Inject] private ToastService ToastService { get; init; }
    private ClientDto? _client;
    protected override async Task OnParametersSetAsync()
    {
        _client = (await ClientsService.GetClientById(ClientId, UserState.Value.User.Token)).Client;
        await base.OnParametersSetAsync();
    }

    private async Task<GridDataProviderResult<ClientDto>> ClientDataProvider(GridDataProviderRequest<ClientDto> request)
    {
        try
        {
            var result = await ClientsService.GetMatchingClients(ClientId, RequirementId,
                request.PageNumber, request.PageSize, UserState.Value.User.Token);
            
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