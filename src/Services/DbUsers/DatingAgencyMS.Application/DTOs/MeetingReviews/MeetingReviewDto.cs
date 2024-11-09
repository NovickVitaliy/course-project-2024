namespace DatingAgencyMS.Application.DTOs.MeetingReviews;

public record MeetingReviewDto(
    int Id,
    int InviterScore,
    string InviterReview,
    int InviteeScore,
    string InviteeReview,
    int MeetingId
);