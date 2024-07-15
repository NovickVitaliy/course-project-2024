using DatingAgencyMS.Client.Models.DTOs.User.Requests;
using DatingAgencyMS.Client.Models.DTOs.User.Responses;
using Refit;

namespace DatingAgencyMS.Client.Services;

public interface IUsersService
{
    [Post("/users")]
    Task<CreateUserResponse> CreateUser(CreateUserRequest request, [Authorize] string token);

    [Get("/users")]
    Task<GetUsersResponse> GetUsers([Query]GetUsersRequest request, [Authorize] string token);
}