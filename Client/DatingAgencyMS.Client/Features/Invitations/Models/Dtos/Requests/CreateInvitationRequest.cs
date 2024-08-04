using System.ComponentModel.DataAnnotations;

namespace DatingAgencyMS.Client.Features.Invitations.Models.Dtos.Requests;

public class CreateInvitationRequest
{
    [Required(ErrorMessage = "Id того хто запрошує не може бути пустим")]
    [Range(0, int.MaxValue)]
    public int InviterId { get; set; }
    
    [Required(ErrorMessage = "Id того кого запрошують не може бути пустим")]
    [Range(0, int.MaxValue)]
    public int InviteeId { get; set; }

    [Required(ErrorMessage = "Місце зустрічі не може бути пустим")]
    public string Location { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Час зустрічі не може бути пустим")]
    public DateTime DateOfMeeting { get; set; }
    
    public string RequestedBy { get; set; }
}