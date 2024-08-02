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

public partial class RegisteredByPeriod : ComponentBase
{
    [Parameter]
    public string Period { get; init; }

    private Constants.RegisteredByPeriod? _periodAsEnum;
    [Inject] private NavigationManager NavigationManager { get; init; }
    [Inject] private ToastService ToastService { get; init; } 
    [Inject] private IClientsService ClientsService { get; init; }
    [Inject] private IState<UserState> UserState { get; init; }
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
        try
        {
            var clientsRequest = new GetRegisterdClientsByPeriodRequest(_periodAsEnum.Value, request.PageNumber,
                request.PageSize, UserState.Value.User.Login);
            var result = await ClientsService.GetRegisteredClientsByPeriod(clientsRequest, UserState.Value.User.Token);

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