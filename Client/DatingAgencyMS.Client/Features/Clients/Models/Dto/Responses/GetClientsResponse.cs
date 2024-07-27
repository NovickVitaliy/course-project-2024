using DatingAgencyMS.Client.Models.Core;

namespace DatingAgencyMS.Client.Features.Clients.Models.Dto.Responses;

public record GetClientsResponse(IReadOnlyList<ClientDto> Clients, long TotalCount);