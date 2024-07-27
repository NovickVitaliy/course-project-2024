using BlazorBootstrap;
using Common.Filtering.Filters;
using Common.Filtering.Filters.NumberFilters;
using Common.Filtering.FiltersOptions;
using Common.Filtering.Pagination;
using Common.Filtering.Sorting;
using DatingAgencyMS.Client.Features.Clients.Models.Dto.Requests;
using DatingAgencyMS.Client.Features.Clients.Services;
using DatingAgencyMS.Client.Helpers;
using DatingAgencyMS.Client.Models.Core;
using DatingAgencyMS.Client.Store.UserUseCase;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DatingAgencyMS.Client.Features.Clients.Components;

public partial class ClientsList : ComponentBase
{
    [Parameter, EditorRequired] public LoggedInUser LoggedInUser { get; init; }

    [Inject] private IJSRuntime JsRuntime { get; init; }

    [Inject] private IClientsService ClientsService { get; init; }


    private async Task<GridDataProviderResult<ClientDto>> ClientDataProvider(GridDataProviderRequest<ClientDto> request)
    {
        var clientsRequest = BuildRequest(request);
        var response = await ClientsService.GetClients(clientsRequest, LoggedInUser.Token);
        await JsRuntime.InvokeVoidAsync("console.log", clientsRequest);

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
        var sexualOrientationFilter = request.Filters.GetStringFilter("SexualOrientation");
        var registrationNumberFilter = request.Filters.GetStringFilter("RegistrationNumber");
        var registeredOnFilter = request.Filters.GetDateOnlyFilter("RegisteredOn");
        var ageFilter = request.Filters.GetIntegerFilter("Age");
        var heightFilter = request.Filters.GetIntegerFilter("Height");
        var weightFilter = request.Filters.GetIntegerFilter("Weight");
        var zodiasSignFilter = request.Filters.GetStringFilter("ZodiacSign");
        var description = request.Filters.GetStringFilter("Description");
        var sorting = request.Sorting.FirstOrDefault();
        SortingInfo? sortingInfo = null;
        if (sorting is not null)
        {
            sortingInfo = new SortingInfo(sorting.SortString, sorting.SortDirection == SortDirection.Ascending
                ? "ASC"
                : "DESC");
        }

        var paginationInfo = new PaginationInfo(request.PageNumber, request.PageSize);
        return new GetClientsRequest(idFilter, firstNameFilter, lastNameFilter, genderFilter, sexualOrientationFilter,
            registrationNumberFilter,
            registeredOnFilter,
            ageFilter, heightFilter, weightFilter, zodiasSignFilter, description, sortingInfo, paginationInfo, LoggedInUser.Login);
    }
}