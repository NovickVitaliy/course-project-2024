namespace DatingAgencyMS.Domain.Models.Business;

public class MeetingReview
{
    public required int Id { get; init; }
    public int InviterScore { get; init; }
    public string InviterReview { get; init; } = string.Empty;
    public int InviteeScore { get; init; }
    public string InviteeReview { get; init; } = string.Empty;
}