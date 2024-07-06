using DbUsers.Application.Contracts;
using DbUsers.Application.Shared;

namespace DbUsers.Infrastructure.Services;

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
}