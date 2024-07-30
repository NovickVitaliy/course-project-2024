using BlazorBootstrap;
using Common.Filtering.Pagination;
using DatingAgencyMS.Client.Features.Clients.Models.Dto.Requests;
using DatingAgencyMS.Client.Features.Clients.Services;
using DatingAgencyMS.Client.Models.Core;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace DatingAgencyMS.Client.Features.Clients.Pages;

public partial class ClientByYearQuarters : ComponentBase
{
    [SupplyParameterFromQuery] private int Year { get; init; }

    [Inject] private IClientsService ClientsService { get; init; }

    [Inject] private IState<UserState> UserState { get; init; }

    private object _lockObj = new();

    private async Task<GridDataProviderResult<ClientDto>> GetClientsByYearQuarter(
        GridDataProviderRequest<ClientDto> request, int quarter)
    {
        var paginationInfo = new PaginationInfo(request.PageNumber, request.PageSize);
        var clientsRequest =
            new GetClientsByYearQuarterRequest(paginationInfo, Year, quarter, UserState.Value.User.Login);
        var response = await ClientsService.GetClientsByYearQuarter(clientsRequest, UserState.Value.User.Token);

        return new GridDataProviderResult<ClientDto>()
        {
            Data = response.Clients,
            TotalCount = (int?)response.TotalCount
        };
    }
}