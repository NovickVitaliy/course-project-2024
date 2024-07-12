using DatingAgencyMS.Domain.Models;

namespace DatingAgencyMS.Application.DTOs.UserManagement;

public record AssignNewRoleRequest(string Login, DbRoles NewRole, string RequestedBy);