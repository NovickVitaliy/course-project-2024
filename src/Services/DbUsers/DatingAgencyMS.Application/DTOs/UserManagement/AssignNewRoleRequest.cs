namespace DatingAgencyMS.Application.DTOs.UserManagement;

public record AssignNewRoleRequest(string Login, string NewRole, string RequestedBy);