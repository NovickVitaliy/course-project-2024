namespace DatingAgencyMS.Application.DTOs.MeetingReviews.Requests;

public record UpdateMeetingReviewRequest(
    int Id,
    int InviterScore,
    string InviterReview,
    int InviteeScore,
    string InviteeReview
);