namespace DatingAgencyMS.Client.Features.MeetingReviews.Models;

public record MeetingReviewDto(
    int Id,
    int InviterScore,
    string InviterReview,
    int InviteeScore,
    string InviteeReview,
    int MeetingId);