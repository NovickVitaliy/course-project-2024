using DatingAgencyMS.Application.DTOs;
using DatingAgencyMS.Application.DTOs.UserManagement;
using DatingAgencyMS.Application.Shared;

namespace DatingAgencyMS.Application.Contracts;

public interface IUserManager
{
    Task<ServiceResult<long>> CreateUser(CreateUserRequest request);
    Task<ServiceResult<bool>> DeleteUser(DeleteUserRequest request);
    Task<ServiceResult<bool>> AssignNewRole(AssignNewRoleRequest request);
}