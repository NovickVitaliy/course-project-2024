using System.ComponentModel.DataAnnotations;
using DatingAgencyMS.Client.Attributes.ValidationAttributes;
using DatingAgencyMS.Client.Models.Core;

namespace DatingAgencyMS.Client.Models.DTOs.User.Requests;

public class CreateUserRequest
{
    [RestrictedLogin]
    [Required(ErrorMessage = "Логін не може бути пустим")]
    public string Login { get; set; }
    
    [Required(ErrorMessage = "Пароль не може бути пустим")]
    public string Password { get; set; }
    
    [Required(ErrorMessage = "Роль не може бути пустою")]
    public DbRoles? Role { get; set; }
    
    public string RequestedBy { get; set; }
}