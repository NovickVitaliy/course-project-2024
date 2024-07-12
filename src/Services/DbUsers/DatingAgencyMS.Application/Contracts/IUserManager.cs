using DatingAgencyMS.Application.DTOs;
using DatingAgencyMS.Application.DTOs.UserManagement;
using DatingAgencyMS.Application.Shared;
using DatingAgencyMS.Domain.Models;

namespace DatingAgencyMS.Application.Contracts;

public interface IUserManager
{
    Task<ServiceResult<long>> CreateUser(CreateUserRequest request);
    Task<ServiceResult<bool>> DeleteUser(DeleteUserRequest request);
    Task<ServiceResult<bool>> AssignNewRole(AssignNewRoleRequest request);
    Task<ServiceResult<DbRoles>> GetUserRole(string login);
}