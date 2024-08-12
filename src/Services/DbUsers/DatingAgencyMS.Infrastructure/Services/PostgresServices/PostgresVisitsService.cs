using System.Data.Common;
using Common.Filtering.Pagination;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.DTOs.Visits;
using DatingAgencyMS.Application.DTOs.Visits.Requests;
using DatingAgencyMS.Application.DTOs.Visits.Responses;
using DatingAgencyMS.Application.Shared;
using DatingAgencyMS.Infrastructure.Extensions;
using DatingAgencyMS.Infrastructure.Helpers;
using System.Data;

namespace DatingAgencyMS.Infrastructure.Services.PostgresServices;

public class PostgresVisitsService : IVisitsService
{
    private readonly IDbManager _dbManager;

    public PostgresVisitsService(IDbManager dbManager)
    {
        _dbManager = dbManager;
    }

    public async Task<ServiceResult<GetVisitsResponse>> GetVisits(GetVisitsRequest request)
    {
        List<VisitDto> visits = [];
        var connection = await _dbManager.GetConnectionOrThrow();
        await using var transaction = await connection.BeginTransactionAsync();
        try
        {
            var sql = BuildSelectSqlForVisits(request);
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            cmd.CommandText = sql.FullSql;
            var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                visits.Add(await ReadSingleVisit(reader));
            }

            await reader.CloseAsync();
            
            cmd.CommandText = $"SELECT COUNT(*) FROM meetingvisit {sql.ConditionSql}";
            var count = (long?)await cmd.ExecuteScalarAsync();

            await transaction.CommitAsync();

            return ServiceResult<GetVisitsResponse>.Ok(new GetVisitsResponse(visits, count.Value));

        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return ServiceResult<GetVisitsResponse>.BadRequest(e.ToString());
        }
    }

    private async Task<VisitDto> ReadSingleVisit(DbDataReader reader)
    {
        var id = reader.GetInt32("id");
        var clientId = reader.GetInt32("client_id");
        var meetingId = reader.GetInt32("meeting_id");
        var visited = reader.GetBoolean("visited");

        return new VisitDto(id, clientId, meetingId, visited);
    }

    private (string FullSql, string ConditionSql) BuildSelectSqlForVisits(GetVisitsRequest request)
    {
        const string select = "SELECT * FROM meetingvisit ";
        const string initialCondition = "WHERE 1=1 ";
        var idCondition = request.IdFilter.BuildConditionForInteger("id");
        var clientIdCondition = request.ClientIdFilter.BuildConditionForInteger("client_id");
        var meetingIdCondition = request.MeetingIdFilter.BuildConditionForInteger("meeting_id");
        var visitedCondition = request.VisitedFilter.BuildConditionForBoolean("visited");
        var sorting = request.SortingInfo.BuildSortingString();
        var pagination = request.PaginationInfo.ToPostgreSqlPaginationString();

        var conditionSql = string.Concat(initialCondition, idCondition, clientIdCondition, meetingIdCondition, visitedCondition);
        var fullSql = string.Concat(select, conditionSql, sorting, pagination);

        return (fullSql, conditionSql);
    }
}