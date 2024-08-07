using DatingAgencyMS.Client.Models.Core;

namespace DatingAgencyMS.Client.Features.Meetings.Models;

public record MeetingDto(
    int MeetingId,
    DateTime Date,
    int InviterId,
    int InviteeId,
    string Location,
    MeetingResult Result);