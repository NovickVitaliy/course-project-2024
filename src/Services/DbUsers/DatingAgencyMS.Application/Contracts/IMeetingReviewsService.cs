using DatingAgencyMS.Application.DTOs.MeetingReviews;
using DatingAgencyMS.Application.DTOs.MeetingReviews.Requests;
using DatingAgencyMS.Application.DTOs.MeetingReviews.Responses;
using DatingAgencyMS.Application.Shared;

namespace DatingAgencyMS.Application.Contracts;

public interface IMeetingReviewsService
{
    Task<ServiceResult<int>> CreateMeetingReviewAsync(CreateMeetingReviewRequest request);
    Task<ServiceResult<GetMeetingReviewsResponse>> GetMeetingReviewsAsync(GetMeetingReviewsRequest request);
    Task<ServiceResult<MeetingReviewDto>> GetMeetingReviewByIdAsync(int id);
    Task<ServiceResult<bool>> UpdateMeetingReviewAsync(UpdateMeetingReviewRequest request);
    Task<ServiceResult<bool>> DeleteMeetingReviewAsync(int id);
}