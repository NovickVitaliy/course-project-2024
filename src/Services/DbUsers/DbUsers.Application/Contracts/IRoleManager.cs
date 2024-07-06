using DbUsers.Application.Shared;

namespace DbUsers.Application.Contracts;

public interface IRoleManager
{
    Task<ServiceResult<int>> CreateRole(string roleName);
    Task<ServiceResult<int>> GetRoleId(string roleName);
    Task<ServiceResult<int>> DeleteRole(string roleName);
}