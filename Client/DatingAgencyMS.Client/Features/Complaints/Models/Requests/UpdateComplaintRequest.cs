using System.ComponentModel.DataAnnotations;

namespace DatingAgencyMS.Client.Features.Complaints.Models.Requests;

public class UpdateComplaintRequest
{
    public int ComplaintId { get; set; }
    
    [Required(ErrorMessage = "Текст скарги має бути вказаний")]
    [StringLength(50, ErrorMessage = "Текст скарги має бути меншим за 50 симолів")]
    public string Text { get; set; } = null!;
}