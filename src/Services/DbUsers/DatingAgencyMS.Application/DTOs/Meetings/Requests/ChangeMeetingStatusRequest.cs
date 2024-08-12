using DatingAgencyMS.Domain.Models.Business;

namespace DatingAgencyMS.Application.DTOs.Meetings.Requests;

public record ChangeMeetingStatusRequest(
    MeetingResult MeetingStatus,
    bool InviterVisited,
    bool InviteeVisited,
    int MeetingId = 0);