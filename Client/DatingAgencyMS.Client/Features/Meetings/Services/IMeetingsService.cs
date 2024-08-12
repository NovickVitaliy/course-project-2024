using DatingAgencyMS.Client.Features.Meetings.Models.Dtos.Requests;
using DatingAgencyMS.Client.Features.Meetings.Models.Dtos.Responses;
using Microsoft.AspNetCore.Mvc;
using Refit;

namespace DatingAgencyMS.Client.Features.Meetings.Services;

public interface IMeetingsService
{
    [Get("/meetings")]
    Task<GetMeetingsResponse> GetMeetings(GetMeetingsRequest request, [Authorize] string token);

    [Get("/meetings/planned")]
    Task<GetMeetingsResponse> GetPlannedMeetingsForPeriod([FromQuery] GetPlannedMeetingsForPeriodRequest request, [Authorize] string token);
    
    [Put("/meetings/{meetingId}/change-status")]
    Task ChangeMeetingStatus(int meetingId, ChangeMeetingStatusRequest request, [Authorize] string token);
}