using DbUsers.API.Models.ServiceResults;
using DbUsers.API.Services.Contracts;

namespace DbUsers.API.Services.Implementations;

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