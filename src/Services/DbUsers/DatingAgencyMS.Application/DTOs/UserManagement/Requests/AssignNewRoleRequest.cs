using DatingAgencyMS.Domain.Models;
using DatingAgencyMS.Domain.Models.DbManagement;

namespace DatingAgencyMS.Application.DTOs.UserManagement.Requests;

public record AssignNewRoleRequest(string Login, DbRoles OldRole, DbRoles NewRole);