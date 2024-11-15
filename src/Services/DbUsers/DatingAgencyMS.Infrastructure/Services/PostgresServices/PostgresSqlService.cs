using System.Text;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.DTOs.SqlQueries;
using DatingAgencyMS.Application.Shared;

namespace DatingAgencyMS.Infrastructure.Services.PostgresServices;

public class PostgresSqlService : ISqlQueryService
{
    private readonly IDbManager _dbManager;
    
    public PostgresSqlService(IDbManager dbManager)
    {
        _dbManager = dbManager;
    }
    
    public async Task<ServiceResult<SqlQueryResponse>> RunSqlAsync(SqlQueryRequest request)
    {
        var connection = await _dbManager.GetConnectionOrThrow();

        try
        {
            await using var cmd = connection.CreateCommand();
            cmd.Connection = connection;
            cmd.CommandText = request.Sql;
            string sqlOption = request.Sql.Split(' ')[0].ToLowerInvariant();
            switch (sqlOption)
            {
                case "insert":
                    await cmd.ExecuteNonQueryAsync();
                    return ServiceResult<SqlQueryResponse>.Ok(new SqlQueryResponse(["Insert query run successfully"]));
                case "select":
                    List<string> results = [];
                    var reader = await cmd.ExecuteReaderAsync();

                    var columns = Enumerable.Range(0, reader.FieldCount)
                        .Select(reader.GetName)
                        .ToList();

                    var cols = string.Join(" | ", columns);
                    results.Add(cols);
                    while (await reader.ReadAsync())
                    {
                        var rowValues = new List<string>();

                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            var value = reader.IsDBNull(i) ? "NULL" : reader[i].ToString();
                            rowValues.Add(value + " | ");
                        }

                        results.Add(string.Join("\t", rowValues));
                    }

                    return ServiceResult<SqlQueryResponse>.Ok(new SqlQueryResponse(results.ToArray()));
                case "update":
                    await cmd.ExecuteNonQueryAsync();
                    return ServiceResult<SqlQueryResponse>.Ok(new SqlQueryResponse(["Update query run successfuly"]));
                case "delete":
                    await cmd.ExecuteNonQueryAsync();
                    return ServiceResult<SqlQueryResponse>.Ok(new SqlQueryResponse(["Delete query run successfuly"]));
                default:
                    return ServiceResult<SqlQueryResponse>.Ok(new SqlQueryResponse(["Unknown type of query"]));
            }
        }
        catch (Exception)
        {
            return ServiceResult<SqlQueryResponse>.BadRequest("Сталася помилка при запуску заданого SQL запиту. Перевірьте будь ласка чи запит був написаний правильно.");
        }
    }
}