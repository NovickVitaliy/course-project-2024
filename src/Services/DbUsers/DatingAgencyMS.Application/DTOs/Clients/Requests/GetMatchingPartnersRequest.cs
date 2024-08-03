namespace DatingAgencyMS.Application.DTOs.Clients.Requests;

public record GetMatchingPartnersRequest(int ClientId, int RequirementId, string RequestedBy, int PageNumber, int PageSize);