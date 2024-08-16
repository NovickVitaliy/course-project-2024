using DatingAgencyMS.Application.DTOs.Clients;
using DatingAgencyMS.Application.DTOs.Clients.Requests;
using DatingAgencyMS.Application.DTOs.Clients.Responses;
using DatingAgencyMS.Application.Shared;

namespace DatingAgencyMS.Application.Contracts;

public interface IClientsService
{
    Task<ServiceResult<GetClientsResponse>> GetClients(GetClientsRequest request);
    Task<ServiceResult<CreateClientResponse>> CreateClient(CreateClientRequest request);
    Task<ServiceResult<bool>> DeleteClient(int clientId, string requestedBy);
    Task<ServiceResult<bool>> UpdateClient(int clientId, UpdateClientRequest request);
    Task<ServiceResult<GetClientResponse>> GetClientById(GetClientRequest getClientRequest);
    Task<ServiceResult<long>> GetCountOfClientsWhoDeclinedService(string requestedBy);
    Task<ServiceResult<GetClientsResponse>> GetClientsByYearQuarter(GetClientsByYearQuarterRequest request);
    Task<ServiceResult<GetClientsResponse>> GetRegisteredClientsByPeriod(GetClientsByTimePeriodRequest request);
    Task<ServiceResult<bool>> DeleteClientsWhoDeclinedService(string requestedBy);
    Task<ServiceResult<GetClientsResponse>> GetMatchingPartners(GetMatchingPartnersRequest request);
    Task<ServiceResult<GetClientsResponse>> GetClientsWhoDidNotSkipAnyMeeting(int pageNumber, int pageSize);
}