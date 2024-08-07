using DatingAgencyMS.Client.Features.Meetings.Models.Dtos.Requests;
using DatingAgencyMS.Client.Features.Meetings.Models.Dtos.Responses;
using Refit;

namespace DatingAgencyMS.Client.Features.Meetings.Services;

public interface IMeetingsService
{
    [Get("/meetings")]
    Task<GetMeetingsResponse> GetMeetings(GetMeetingsRequest request, [Authorize] string token);
}