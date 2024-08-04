namespace DatingAgencyMS.Client.Features.Invitations.Models.Dtos.Responses;

public record GetinvitationsResponse(IReadOnlyList<InvitationDto> Invitations, long TotalCount);