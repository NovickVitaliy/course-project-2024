using BlazorBootstrap;
using DatingAgencyMS.Client.Extensions;
using DatingAgencyMS.Client.Features.Clients.Services;
using DatingAgencyMS.Client.Models.Core;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Refit;

namespace DatingAgencyMS.Client.Features.Clients.Pages;

public partial class ClientsWhoDidNotSkipAnyMeeting 
{
    [Inject] private ToastService ToastService { get; init; }
    [Inject] private IClientsService ClientsService { get; init; }
    [Inject] private IState<UserState> UserState { get; init; }
    private LoggedInUser? _loggedInUser;

    protected override void OnInitialized()
    {
        _loggedInUser = UserState.Value.User;
        UserState.StateChanged += (_, _) => _loggedInUser = UserState.Value.User;
        base.OnInitialized();
    }

    private async Task<GridDataProviderResult<ClientDto>> ClientDataProvider(GridDataProviderRequest<ClientDto> request)
    {
        if (_loggedInUser == null) return new GridDataProviderResult<ClientDto>();
        try
        {
            var response = await ClientsService.GetClientsWhoDidNotSkipAnyMeeting(request.PageNumber, request.PageSize, _loggedInUser.Token);
            return new GridDataProviderResult<ClientDto>()
            {
                Data = response.Clients,
                TotalCount = (int?)response.TotalCount
            };
        }
        catch (ApiException e)
        {
            var apiError = e.ToApiError();
            ToastService.Notify(new ToastMessage(ToastType.Danger, apiError.Description));
            return new GridDataProviderResult<ClientDto>();
        }
    }
}