namespace DatingAgencyMS.Domain.Models.Business;

public class Client
{
    public int ClientId { get; init; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Sex { get; set; } = string.Empty;
    public string SexualOrientation { get; set; } = string.Empty;
    public string Location { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;
    public DateOnly RegisteredOn { get; set; }
    public int Age { get; set; }
    public int Height { get; set; }
    public int Weight { get; set; }
    public ZodiacSign ZodiacSign { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool HasDeclinedService { get; set; } = false;
}