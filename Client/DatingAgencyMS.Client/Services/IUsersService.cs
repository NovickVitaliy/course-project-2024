using DatingAgencyMS.Client.Models.DTOs.User;
using Refit;

namespace DatingAgencyMS.Client.Services;

public interface IUsersService
{
    [Post("/users")]
    Task<CreateUserResponse> CreateUser(CreateUserRequest request, [Authorize]string token);
}