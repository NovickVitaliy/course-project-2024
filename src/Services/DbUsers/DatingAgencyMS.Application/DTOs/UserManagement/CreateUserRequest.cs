using DatingAgencyMS.Domain.Models;

namespace DatingAgencyMS.Application.DTOs.UserManagement;

public record CreateUserRequest(string Login, string Password, DbRoles Role, string RequestedBy);