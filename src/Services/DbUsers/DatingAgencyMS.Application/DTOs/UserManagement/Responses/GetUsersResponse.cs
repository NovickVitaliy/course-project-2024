namespace DatingAgencyMS.Application.DTOs.UserManagement.Responses;

public record GetUsersResponse(IReadOnlyList<DbUserDto> Users, long TotalCount);