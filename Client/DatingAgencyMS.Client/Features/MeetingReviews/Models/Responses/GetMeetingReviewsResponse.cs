namespace DatingAgencyMS.Client.Features.MeetingReviews.Models.Responses;

public record GetMeetingReviewsResponse(
    MeetingReviewDto[] MeetingReviews,
    long Count);