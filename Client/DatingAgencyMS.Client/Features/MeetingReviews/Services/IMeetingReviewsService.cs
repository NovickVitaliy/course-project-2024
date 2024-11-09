using DatingAgencyMS.Client.Features.MeetingReviews.Models;
using DatingAgencyMS.Client.Features.MeetingReviews.Models.Requests;
using DatingAgencyMS.Client.Features.MeetingReviews.Models.Responses;
using Refit;

namespace DatingAgencyMS.Client.Features.MeetingReviews.Services;

public interface IMeetingReviewsService
{
    [Post("/meeting-reviews")]
    Task CreateMeetingReviewAsync(CreateMeetingReviewRequest request, [Authorize] string token);

    [Get("/meeting-reviews")]
    Task<GetMeetingReviewsResponse> GetMeetingReviewsAsync(GetMeetingReviewsRequest request, [Authorize] string token);

    [Get("/meeting-reviews/{id}")]
    Task<MeetingReviewDto> GetMeetingReviewByIdAsync(int id, [Authorize] string token);

    [Put("/meeting-reviews/{id}")]
    Task UpdateMeetingReviewAsync(int id, UpdateMeetingReviewRequest request, [Authorize] string token);

    [Delete("/meeting-reviews/{id}")]
    Task DeleteMeetingReviewAsync(int id, [Authorize] string token);
}