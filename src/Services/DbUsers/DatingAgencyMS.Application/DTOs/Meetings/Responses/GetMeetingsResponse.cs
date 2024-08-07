namespace DatingAgencyMS.Application.DTOs.Meetings.Responses;

public record GetMeetingsResponse(IReadOnlyList<MeetingDto> Meetings, long TotalCount);