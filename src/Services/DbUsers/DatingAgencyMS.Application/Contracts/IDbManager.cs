using System.Data.Common;
using DatingAgencyMS.Application.Shared;

namespace DatingAgencyMS.Application.Contracts;

public interface IDbManager
{
    Task<DbConnection> GetRootConnection();
    Task<ServiceResult<bool>> TryAccessDb(string login, string password);
    Task<ServiceResult<DbConnection>> GetConnection(string login);
    Task<DbConnection> GetConnectionOrThrow();
    Task<ServiceResult<bool>> CloseConnection(string login);
}