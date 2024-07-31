using BlazorBootstrap;
using Common.Filtering.Filters;
using Common.Filtering.Filters.NumberFilters;
using Common.Filtering.FiltersOptions;
using Common.Filtering.Pagination;
using Common.Filtering.Sorting;
using DatingAgencyMS.Client.Extensions;
using DatingAgencyMS.Client.Features.Clients.Models.Dto.Requests;
using DatingAgencyMS.Client.Features.Clients.Services;
using DatingAgencyMS.Client.Helpers;
using DatingAgencyMS.Client.Models.Core;
using DatingAgencyMS.Client.Store.UserUseCase;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Refit;

namespace DatingAgencyMS.Client.Features.Clients.Components;

public partial class ClientsList : ComponentBase
{
    [Inject] private IState<UserState> UserState { get; init; } 

    [Inject] private IJSRuntime JsRuntime { get; init; }

    [Inject] private IClientsService ClientsService { get; init; }

    [Inject] private ToastService ToastService { get; init; }

    private Grid<ClientDto> _grid;

    private ConfirmDialog _confirmDialog = default!;
    
    private async Task<GridDataProviderResult<ClientDto>> ClientDataProvider(GridDataProviderRequest<ClientDto> request)
    {
        var clientsRequest = BuildRequest(request);
        var response = await ClientsService.GetClients(clientsRequest, UserState.Value.User.Token);

        return new GridDataProviderResult<ClientDto>()
        {
            Data = response.Clients,
            TotalCount = (int?)response.TotalCount
        };
    }

    private GetClientsRequest BuildRequest(GridDataProviderRequest<ClientDto> request)
    {
        var idFilter = request.Filters.GetIntegerFilter("ClientId");
        var firstNameFilter = request.Filters.GetStringFilter("FirstName");
        var lastNameFilter = request.Filters.GetStringFilter("LastName");
        var genderFilter = request.Filters.GetStringFilter("Gender");
        var sexFilter = request.Filters.GetStringFilter("Sex");
        var sexualOrientationFilter = request.Filters.GetStringFilter("SexualOrientation");
        var locationFilter = request.Filters.GetStringFilter("Location");
        var registrationNumberFilter = request.Filters.GetStringFilter("RegistrationNumber");
        var registeredOnFilter = request.Filters.GetDateOnlyFilter("RegisteredOn");
        var ageFilter = request.Filters.GetIntegerFilter("Age");
        var heightFilter = request.Filters.GetIntegerFilter("Height");
        var weightFilter = request.Filters.GetIntegerFilter("Weight");
        var zodiasSignFilter = request.Filters.GetStringFilter("ZodiacSign");
        var description = request.Filters.GetStringFilter("Description");
        var hasDeclinedServiceFilter = request.Filters.GetBooleanFilter("HasDeclinedService");
        var sorting = request.Sorting.FirstOrDefault();
        SortingInfo? sortingInfo = null;
        if (sorting is not null)
        {
            sortingInfo = new SortingInfo(sorting.SortString, sorting.SortDirection == SortDirection.Ascending
                ? "ASC"
                : "DESC");
        }
        


        var paginationInfo = new PaginationInfo(request.PageNumber, request.PageSize);
        return new GetClientsRequest(idFilter, firstNameFilter, lastNameFilter, genderFilter, sexFilter, sexualOrientationFilter, locationFilter,
            registrationNumberFilter,
            registeredOnFilter,
            ageFilter, heightFilter, weightFilter, zodiasSignFilter, description, hasDeclinedServiceFilter, sortingInfo, paginationInfo, UserState.Value.User.Login);
    }

    private async Task DeleteClient(int clientId)
    {
        var confirmation = await _confirmDialog.ShowAsync(
            title:$"Ви сравді хочете видалити клієнта з Id - {clientId}?",
            message1:"Це видалить запис з БД. Ця дія незворотня.",
            confirmDialogOptions: new ConfirmDialogOptions()
            {
                YesButtonText = "Видалити",
                YesButtonColor = ButtonColor.Danger,
                NoButtonText = "Назад",
                NoButtonColor = ButtonColor.Secondary
            });

        if (confirmation)
        {
            try
            {
                await ClientsService.DeleteClient(clientId, UserState.Value.User.Token);
                ToastService.Notify(new ToastMessage(ToastType.Success, $"Клієнта з Id - {clientId} було успішно видалено"));
                await _grid.RefreshDataAsync();
            }
            catch (ApiException e)
            {
                var apiError = e.ToApiError();
                ToastService.Notify(new ToastMessage(ToastType.Danger, apiError.Description));
            }
        }
    }
}