using System.ComponentModel.DataAnnotations;

namespace DatingAgencyMS.Client.Models.DTOs.Auth;

public class ForgotPasswordRequest
{
    [Required]
    public string Login { get; set; }
}