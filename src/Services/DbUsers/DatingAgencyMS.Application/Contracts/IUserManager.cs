using DatingAgencyMS.Application.DTOs;
using DatingAgencyMS.Application.Shared;

namespace DatingAgencyMS.Application.Contracts;

public interface IUserManager
{
    Task<ServiceResult<int>> CreateUser(CreateUserRequest request);
    Task<ServiceResult<bool>> DeleteUser(string login);
    Task<ServiceResult<bool>> AssignNewRole(AssignNewRoleRequest request);
}