using System.ComponentModel.DataAnnotations;

namespace DatingAgencyMS.Client.Features.ClientRatings.Models.Requests;

public class UpdateClientRatingRequest
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Оцінка повинна бути вказаний")]
    [Range(0, 10, ErrorMessage = "Оцінка повинна бути в межах від 0 до 10")]
    public int Rating { get; set; }
   
    [Required(ErrorMessage = "Відгук повинен бути вказаний")]
    [StringLength(50, ErrorMessage = "Відгук повинен бути менше 50 символів")]
    public string Comment { get; set; } = null!;
}