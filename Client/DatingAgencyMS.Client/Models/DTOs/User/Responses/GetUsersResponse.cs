namespace DatingAgencyMS.Client.Models.DTOs.User.Responses;

public record GetUsersResponse(IReadOnlyList<DbUser> Users, long TotalCount);