namespace DatingAgencyMS.Domain.Models.Business;

public class AdditionalContacts
{
    public int Id { get; init; }
    public int ClientId { get; init; }
    public string? FirstPhoneNumber { get; set; } 
    public string? SecondPhoneNumber { get; set; }
    public string? Telegram { get; set; }
    public string? Facebook { get; set; }
    public string? Instagram { get; set; }
    public string? TikTok { get; set; }
}