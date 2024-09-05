using System.ComponentModel.DataAnnotations;

namespace DatingAgencyMS.Application.Options;

public class PasswordEncryptionOptions
{
    public const string Position = "PasswordEncryptionOptions";
    
    [Required]
    public string Key { get; init; }
    
    [Required]
    public string Iv { get; init; }
}