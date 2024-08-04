namespace DatingAgencyMS.Client.Features.Invitations.Models.Dtos;

public record InvitationDto(
    int InvitationId,
    int InviterId,
    int InviteeId,
    string Location,
    DateTime DateOfMeeting,
    DateOnly CreatedOn,
    DateOnly ActiveTo,
    bool IsAccepted);