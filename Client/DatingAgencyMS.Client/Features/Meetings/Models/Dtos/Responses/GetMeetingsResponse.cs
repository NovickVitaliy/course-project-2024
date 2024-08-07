namespace DatingAgencyMS.Client.Features.Meetings.Models.Dtos.Responses;

public record GetMeetingsResponse(IReadOnlyList<MeetingDto> Meetings, long TotalCount);