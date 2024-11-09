namespace DatingAgencyMS.Application.DTOs.MeetingReviews.Responses;

public record GetMeetingReviewsResponse(
    MeetingReviewDto[] MeetingReviews,
    long Count);