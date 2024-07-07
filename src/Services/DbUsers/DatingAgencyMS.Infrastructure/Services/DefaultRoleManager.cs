using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.DTOs;
using DatingAgencyMS.Application.Shared;

namespace DatingAgencyMS.Infrastructure.Services;

public class DefaultRoleManager : IRoleManager
{
    public Task<ServiceResult<int>> CreateRole(string roleName)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult<int>> GetRoleId(string roleName)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult<int>> DeleteRole(string roleName)
    {
        throw new NotImplementedException();
    }

    public Task<ServiceResult<IReadOnlyList<RoleDto>>> GetRoles()
    {
        throw new NotImplementedException();
    }
}