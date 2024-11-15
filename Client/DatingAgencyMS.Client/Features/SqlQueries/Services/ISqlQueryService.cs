using DatingAgencyMS.Client.Features.SqlQueries.Models;
using Refit;

namespace DatingAgencyMS.Client.Features.SqlQueries.Services;

public interface ISqlQueryService
{
    [Post("/sql-queries")]
    Task<SqlQueryResponse> RunSqlQueryAsync(SqlQueryRequest request, [Authorize] string token);
}