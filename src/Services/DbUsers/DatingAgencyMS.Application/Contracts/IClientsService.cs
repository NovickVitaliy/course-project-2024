using DatingAgencyMS.Application.DTOs.Clients;
using DatingAgencyMS.Application.DTOs.Clients.Requests;
using DatingAgencyMS.Application.DTOs.Clients.Responses;
using DatingAgencyMS.Application.Shared;

namespace DatingAgencyMS.Application.Contracts;

public interface IClientsService
{
    Task<ServiceResult<GetClientsResponse>> GetClients(GetClientsRequest request);
    Task<ServiceResult<CreateClientResponse>> CreateClient(CreateClientRequest request);
}