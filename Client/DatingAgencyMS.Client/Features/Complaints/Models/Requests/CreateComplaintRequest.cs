using System.ComponentModel.DataAnnotations;

namespace DatingAgencyMS.Client.Features.Complaints.Models.Requests;

public class CreateComplaintRequest
{
    [Required(ErrorMessage = "Id того хто скаржиться має бути вказаний")]
    [Range(0, int.MaxValue, ErrorMessage = "Id того хто скаржиться має бути більшим за 0")]
    public int ComplainantId { get; set; }
    
    [Required(ErrorMessage = "Id того на кого скаржаться має бути вказаний")]
    [Range(0, int.MaxValue, ErrorMessage = "Id того на кого скаржаться має бути більшим за 0")]
    public int ComplaineeId { get; set; }

    [Required(ErrorMessage = "Текст скарги має бути вказаний")]
    [StringLength(50, ErrorMessage = "Текст скарги має бути меншим за 50 симолів")]
    public string Text { get; set; } = null!;
}