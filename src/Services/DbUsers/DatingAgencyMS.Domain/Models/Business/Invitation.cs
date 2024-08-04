namespace DatingAgencyMS.Domain.Models.Business;

public class Invitation
{
    public int InvitationId { get; init; }
    public int InviterId { get; init; }
    public int InviteeId { get; init; }
    public string Location { get; init; } = string.Empty;
    public DateTime DateOfMeeting { get; init; }
    public DateOnly CreatedOn { get; init; }
    public DateOnly ActiveTo { get; init; }
    public bool IsAccepted { get; set; }
}