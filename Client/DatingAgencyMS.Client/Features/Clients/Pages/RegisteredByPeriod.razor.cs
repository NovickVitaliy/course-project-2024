using BlazorBootstrap;
using DatingAgencyMS.Client.Constants;
using DatingAgencyMS.Client.Extensions;
using DatingAgencyMS.Client.Features.Clients.Models.Dto.Requests;
using DatingAgencyMS.Client.Features.Clients.Services;
using DatingAgencyMS.Client.Models.Core;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Refit;

namespace DatingAgencyMS.Client.Features.Clients.Pages;

public partial class RegisteredByPeriod
{
    [Parameter] public string Period { get; init; }
    private Constants.RegisteredByPeriod? _periodAsEnum;
    [Inject] private NavigationManager NavigationManager { get; init; }
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

    protected override void OnParametersSet()
    {
        if (Enum.TryParse<Constants.RegisteredByPeriod>(Period, true, out var period))
        {
            _periodAsEnum = period;
        }
        else
        {
            NavigationManager.NavigateTo("/tables/clients");
        }
    }

    private async Task<GridDataProviderResult<ClientDto>> GetRegisteredClientsByPeriodDataProvider(GridDataProviderRequest<ClientDto> request)
    {
        if (_loggedInUser is null) return new GridDataProviderResult<ClientDto>();
        try
        {
            var clientsRequest = new GetRegisterdClientsByPeriodRequest(_periodAsEnum.Value, request.PageNumber,
                request.PageSize, _loggedInUser.Login);
            var result = await ClientsService.GetRegisteredClientsByPeriod(clientsRequest, _loggedInUser.Token);

            return new GridDataProviderResult<ClientDto>()
            {
                Data = result.Clients,
                TotalCount = (int?)result.TotalCount
            };
        }
        catch (ApiException e)
        {
            var apiError = e.ToApiError();
            ToastService.Notify(new ToastMessage(ToastType.Danger, apiError.Description));
            return new GridDataProviderResult<ClientDto>()
            {
                Data = [],
                TotalCount = 0
            };
        }
    }
}