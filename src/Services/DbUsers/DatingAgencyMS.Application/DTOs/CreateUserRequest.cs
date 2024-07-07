namespace DatingAgencyMS.Application.DTOs;

public record CreateUserRequest(string Login, string Password, string Role);