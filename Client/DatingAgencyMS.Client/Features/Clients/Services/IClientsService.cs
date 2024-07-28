using DatingAgencyMS.Client.Features.Clients.Models.Dto.Requests;
using DatingAgencyMS.Client.Features.Clients.Models.Dto.Responses;
using Refit;

namespace DatingAgencyMS.Client.Features.Clients.Services;

public interface IClientsService
{
    [Get("/clients")]
    Task<GetClientsResponse> GetClients([Query]GetClientsRequest request, [Authorize] string token);
    
    [Post("/clients")]
    Task<CreateClientResponse> CreateClient(CreateClientRequest request, [Authorize] string token);

    [Delete("/clients/{clientId}")]
    Task DeleteClient(int clientId, [Authorize] string token);
}