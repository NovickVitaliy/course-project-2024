using DatingAgencyMS.Domain.Models;

namespace DatingAgencyMS.Application.DTOs.UserManagement.Requests;

public record AssignNewRoleRequest(string Login, DbRoles OldRole, DbRoles NewRole, string RequestedBy);