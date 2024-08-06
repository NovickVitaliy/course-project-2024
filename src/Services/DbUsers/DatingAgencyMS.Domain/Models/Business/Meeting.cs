namespace DatingAgencyMS.Domain.Models.Business;

public class Meeting
{
    public int MeetingId { get; init; }
    public DateTime Date { get; init; }
    public int InviterId { get; init; }
    public int InviteeId { get; init; }
    public string Location { get; init; } = string.Empty;
    public MeetingResult Result { get; init; }
}