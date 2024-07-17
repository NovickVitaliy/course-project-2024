using DatingAgencyMS.Client.Models.DTOs.User.Requests;
using DatingAgencyMS.Client.Models.DTOs.User.Responses;
using Refit;

namespace DatingAgencyMS.Client.Services;

public interface IUsersService
{
    [Post("/users")]
    Task<CreateUserResponse> CreateUser(CreateUserRequest request, [Authorize] string token);

    [Get("/users")]
    Task<GetUsersResponse> GetUsers([Query] GetUsersRequest request, [Authorize] string token);

    [Delete("/users")]
    Task DeleteUser([Query] DeleteUserRequest request, [Authorize] string token);

    [Get("/users/{login}")]
    Task<GetUserResponse> GetUser(string login, [Authorize] string token);
}