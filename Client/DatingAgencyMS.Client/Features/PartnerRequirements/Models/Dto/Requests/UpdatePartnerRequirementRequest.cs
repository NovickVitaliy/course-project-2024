using System.ComponentModel.DataAnnotations;
using DatingAgencyMS.Client.Models.Core;

namespace DatingAgencyMS.Client.Features.PartnerRequirements.Models.Dto.Requests;

public class UpdatePartnerRequirementRequest
{
    public string? Gender { get; set; }
    public string? Sex { get; set; }
    
    [Range(18, 90, ErrorMessage = "Вік має бути в межах від 18 до 90 років")]
    public int? MinAge { get; set; }
    
    [Range(18, 90, ErrorMessage = "Вік має бути в межах від 18 до 90 років")]
    public int? MaxAge { get; set; }
    
    [Range(0, 300, ErrorMessage = "Ріст повинен бути в межах від 0 до 300 см")]
    public int? MinHeight { get; set; }
    
    [Range(0, 300, ErrorMessage = "Ріст повинен бути в межах від 0 до 300 см")]
    public int? MaxHeight { get; set; }
    
    [Range(0, 300, ErrorMessage = "Вага повинна бути в межах від 0 до 300 кг")]
    public int? MinWeight { get; set; }
    
    [Range(0, 300, ErrorMessage = "Вага повинна бути в межах від 0 до 300 кг")]
    public int? MaxWeight { get; set; }
    public ZodiacSign? ZodiacSign { get; set; }
    public string? City { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Ідентифікатор клієнта має бути вказаний.")]
    public int ClientId { get; set; }
    public string RequestedBy { get; set; }
}