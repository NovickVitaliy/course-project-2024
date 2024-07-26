namespace DatingAgencyMS.Domain.Models.Business;

public class FinishedMeeting
{
    public int MeetingId { get; init; }
    public DateTime Date { get; init; }
    public int FirstClientId { get; init; }
    public int SecondClientId { get; init; }
    public string Location { get; init; } = string.Empty;
    public int FirstClientScore { get; init; }
    public string FirstClientReview { get; init; } = string.Empty;
    public int SecondClientScore { get; init; }
    public string SecondClientReview { get; init; } = string.Empty;
    public MeetingResult Result { get; init; }
}