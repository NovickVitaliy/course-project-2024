using DatingAgencyMS.Application.DTOs.Meetings.Requests;
using DatingAgencyMS.Application.Shared;

namespace DatingAgencyMS.Application.Contracts;

public interface IMeetingsService
{
    Task<ServiceResult<int>> CreateMeeting(CreateMeetingRequest request);
}