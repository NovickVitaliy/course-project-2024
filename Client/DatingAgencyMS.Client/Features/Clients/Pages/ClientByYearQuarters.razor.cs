using BlazorBootstrap;
using Common.Filtering.Pagination;
using DatingAgencyMS.Client.Features.Clients.Models.Dto.Requests;
using DatingAgencyMS.Client.Features.Clients.Services;
using DatingAgencyMS.Client.Models.Core;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace DatingAgencyMS.Client.Features.Clients.Pages;

public partial class ClientByYearQuarters
{
    [SupplyParameterFromQuery] private int Year { get; init; }
    [Inject] private IClientsService ClientsService { get; init; }
    [Inject] private IState<UserState> UserState { get; init; }
    private LoggedInUser? _loggedInUser;

    protected override void OnInitialized()
    {
        _loggedInUser = UserState.Value.User;
        UserState.StateChanged += (_, _) =>
        {
            _loggedInUser = UserState.Value.User;
        };
        base.OnInitialized();
    }

    private async Task<GridDataProviderResult<ClientDto>> GetClientsByYearQuarter(
        GridDataProviderRequest<ClientDto> request, int quarter)
    {
        if (_loggedInUser is null) return new();
        var paginationInfo = new PaginationInfo(request.PageNumber, request.PageSize);
        var clientsRequest =
            new GetClientsByYearQuarterRequest(paginationInfo, Year, quarter, _loggedInUser.Login);
        var response = await ClientsService.GetClientsByYearQuarter(clientsRequest, _loggedInUser.Token);

        return new GridDataProviderResult<ClientDto>()
        {
            Data = response.Clients,
            TotalCount = (int?)response.TotalCount
        };
    }
}