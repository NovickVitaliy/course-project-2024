using DatingAgencyMS.Application.Shared;

namespace DatingAgencyMS.Application.Contracts;

public interface IRoleManager
{
    Task<ServiceResult<int>> CreateRole(string roleName);
    Task<ServiceResult<int>> GetRoleId(string roleName);
    Task<ServiceResult<int>> DeleteRole(string roleName);
}