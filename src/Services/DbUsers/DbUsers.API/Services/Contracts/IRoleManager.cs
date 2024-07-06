using DbUsers.API.Models.ServiceResults;

namespace DbUsers.API.Services.Contracts;

public interface IRoleManager
{
    Task<ServiceResult<int>> CreateRole(string roleName);
    Task<ServiceResult<int>> GetRoleId(string roleName);
    Task<ServiceResult<int>> DeleteRole(string roleName);
}