namespace DatingAgencyMS.Application.DTOs.Clients.Responses;

public record GetClientsResponse(IReadOnlyList<ClientDto> Clients, long TotalCount);