using System.ComponentModel.DataAnnotations;

namespace DatingAgencyMS.Client.Models.DTOs.User.Requests;

public class DeleteUserRequest
{
    [Required(ErrorMessage = "Логін не може бути пустим")]
    public string Login { get; set; }
    
    public string RequestedBy { get; set; }
}