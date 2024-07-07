using System.ComponentModel.DataAnnotations;

namespace DatingAgencyMS.Infrastructure.Settings;

public class JwtSettings
{
    public const string ConfigPath = "JwtSettings";
    
    [Required]
    public string Issuer { get; init; }
    
    [Required]
    public string Audience { get; init; }
    
    [Required]
    public int LifeTimeInMinutes { get; init; }
    
    [Required]
    public string Secret { get; init; }
}