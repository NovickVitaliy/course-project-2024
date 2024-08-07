namespace DatingAgencyMS.Application.DTOs.Meetings.Requests;

public record GetPlannedMeetingsForPeriodRequest(int Year, int Month, int PageNumber, int PageSize);