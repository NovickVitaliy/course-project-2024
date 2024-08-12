using DatingAgencyMS.Application.DTOs.Meetings.Requests;
using DatingAgencyMS.Application.DTOs.Meetings.Responses;
using DatingAgencyMS.Application.Shared;

namespace DatingAgencyMS.Application.Contracts;

public interface IMeetingsService
{
    Task<ServiceResult<int>> CreateMeeting(CreateMeetingRequest request);
    Task<ServiceResult<GetMeetingsResponse>> GetMeetings(GetMeetingsRequest request);
    Task<ServiceResult<GetMeetingsResponse>> GetPlannedMeetingsByPeriod(GetPlannedMeetingsForPeriodRequest request);
    Task<ServiceResult<bool>> ChangeMeetingStatus(ChangeMeetingStatusRequest request);
}