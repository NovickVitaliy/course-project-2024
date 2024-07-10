using System.ComponentModel.DataAnnotations;

namespace DatingAgencyMS.Client.Models.DTOs.Auth;

public class LoginRequest
{
    [Required(ErrorMessage = "Логін не може бути пустим")] public string Login { get; set; } = "";

    [Required(ErrorMessage = "Пароль не може бути пустим")] public string Password { get; set; } = "";

    public override string ToString()
    {
        return $"{nameof(Login)}: {Login}, {nameof(Password)}: {Password}";
    }
}