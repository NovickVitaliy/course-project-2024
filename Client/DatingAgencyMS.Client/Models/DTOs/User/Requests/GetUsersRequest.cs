namespace DatingAgencyMS.Client.Models.DTOs.User.Requests;

public record GetUsersRequest(
    int? Id,
    string? Login,
    string? Role,
    int? Page,
    int? Size,
    string? SortBy,
    string? SortDirection,
    string RequestedBy);