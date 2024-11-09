namespace DatingAgencyMS.Application.DTOs.MeetingReviews.Requests;

public record CreateMeetingReviewRequest(
    int InviterScore,
    string InviterReview,
    int InviteeScore,
    string InviteeReview,
    int MeetingId
);