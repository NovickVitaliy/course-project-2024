using DatingAgencyMS.Domain.Models;

namespace DatingAgencyMS.Application.DTOs.UserManagement.Requests;

public record CreateUserRequest(string Login, string Password, DbRoles Role, string RequestedBy);