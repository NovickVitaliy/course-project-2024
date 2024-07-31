namespace DatingAgencyMS.Domain.Models.Business;

public class PartnerRequirements
{
    public int RequirementsId { get; init; }
    public string? Gender { get; set; } = string.Empty;
    public string? Sex { get; set; } = string.Empty;
    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }
    public int? MinHeight { get; set; }
    public int? MaxHeight { get; set; }
    public int? MinWeight { get; set; }
    public int? MaxWeight { get; set; }
    public ZodiacSign? ZodiacSign { get; set; }
    public string? Location { get; set; } = string.Empty;
    public int ClientId { get; set; }
}