namespace DatingAgencyMS.Application.DTOs.Meetings.Requests;

public record CreateMeetingRequest(
    int InviterId,
    int InviteeId,
    string Location,
    DateTime Date);