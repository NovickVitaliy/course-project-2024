using System.ComponentModel.DataAnnotations;

namespace DatingAgencyMS.Client.Features.MeetingReviews.Models.Requests;

public class CreateMeetingReviewRequest
{
    [Required(ErrorMessage = "Оцінка того хто запрошує повинна бути вказана")]
    [Range(0, 10, ErrorMessage = "Оцінка того хто запрошує повинна бути в межах від 0 до 10")]
    public int InviterScore { get; set; }

    [Required(ErrorMessage = "Відгук того, хто запрошує повинен бути вказаний")]
    [StringLength(50, ErrorMessage = "Довжина відгуку повинна бути менше ніж 50 символів")]
    public string InviterReview { get; set; } = null!;
    
    [Required(ErrorMessage = "Оцінка того, кого запросили повинна бути вказана")]
    [Range(0, 10, ErrorMessage = "Оцінка того, кого запросили повинна бути в межах від 0 до 10")]
    public int InviteeScore { get; set; }

    [Required(ErrorMessage = "Відгук того, кого запросили повинен бути вказаний")]
    [StringLength(50, ErrorMessage = "Відгук того, кого запросили повинен бути менше ніж 50 символів")]
    public string InviteeReview { get; set; } = null!;
    
    [Required(ErrorMessage = "Id зустрічі повинен бути вказаний")]
    [Range(0, int.MaxValue, ErrorMessage = "Id зустрічі повинен бути більший за 0")]
    public int MeetingId { get; set; }
}