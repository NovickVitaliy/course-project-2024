using DatingAgencyMS.Application.DTOs.SqlQueries;
using DatingAgencyMS.Application.Shared;

namespace DatingAgencyMS.Application.Contracts;

public interface ISqlQueryService
{
    Task<ServiceResult<SqlQueryResponse>> RunSqlAsync(SqlQueryRequest request);
}