namespace DatingAgencyMS.Application.DTOs.Invitations.Response;

public record GetInvitationsResponse(IReadOnlyList<InvitationDto> Invitations, long TotalCount);