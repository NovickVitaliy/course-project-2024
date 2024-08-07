namespace DatingAgencyMS.Client.Features.Meetings.Models.Dtos.Requests;

public record GetPlannedMeetingsForPeriodRequest(int Year, int Month, int PageNumber, int PageSize);