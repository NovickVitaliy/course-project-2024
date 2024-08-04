namespace DatingAgencyMS.Application.DTOs.Invitations.Requests;

public record CreateInvitationRequest(
    int InviterId,
    int InviteeId,
    string Location,
    DateTime DateOfMeeting,
    string RequestedBy);