namespace DatingAgencyMS.Application.DTOs.UserManagement;

public record CreateUserRequest(string Login, string Password, string Role, string RequestedBy);