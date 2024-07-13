namespace DatingAgencyMS.Application.DTOs.UserManagement.Requests;

public record GetUsersRequest(
    int? Id,
    string? Login,
    string? Role,
    int? Page,
    int? Size,
    string? SortBy,
    string? SortDirection,
    string RequestedBy);
