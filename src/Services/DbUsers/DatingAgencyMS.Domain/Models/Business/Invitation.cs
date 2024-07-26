namespace DatingAgencyMS.Domain.Models.Business;

public class Invitation
{
    public int InvitationId { get; init; }
    public DateTime CreatedOn { get; init; }
    public int InviterId { get; init; }
    public int InviteeId { get; init; }
    public string Location { get; init; } = string.Empty;
    public DateTime Time { get; init; }
}