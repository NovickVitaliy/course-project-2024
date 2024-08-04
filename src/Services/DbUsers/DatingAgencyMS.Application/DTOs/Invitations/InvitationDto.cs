namespace DatingAgencyMS.Application.DTOs.Invitations;

public record InvitationDto(
    int InvitationId,
    int InviterId,
    int InviteeId,
    string Location,
    DateTime DateOfMeeting,
    DateOnly CreatedOn,
    DateOnly ActiveTo,
    bool IsAccepted);