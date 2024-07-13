namespace DatingAgencyMS.Application.DTOs.UserManagement.Requests;

public record DeleteUserRequest(string Login, string RequestedBy);