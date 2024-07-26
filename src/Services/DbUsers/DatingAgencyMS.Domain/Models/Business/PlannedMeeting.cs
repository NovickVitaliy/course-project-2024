namespace DatingAgencyMS.Domain.Models.Business;

public class PlannedMeeting
{
    public int MeetingId { get; init; }
    public DateTime Date { get; set; }
    public int FirstClientId { get; init; }
    public int SecondClientId { get; init; }
    public string Location { get; set; } = string.Empty;
}