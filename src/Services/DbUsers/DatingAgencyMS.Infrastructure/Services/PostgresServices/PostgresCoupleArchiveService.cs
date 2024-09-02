using System.Data;
using System.Data.Common;
using Common.Filtering.Pagination;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.DTOs.CoupleArchive;
using DatingAgencyMS.Application.DTOs.CoupleArchive.Requests;
using DatingAgencyMS.Application.DTOs.CoupleArchive.Responses;
using DatingAgencyMS.Application.Shared;
using DatingAgencyMS.Infrastructure.Extensions;
using DatingAgencyMS.Infrastructure.Helpers;

namespace DatingAgencyMS.Infrastructure.Services.PostgresServices;

public class PostgresCoupleArchiveService : ICoupleArchiveService
{
    private readonly IDbManager _dbManager;

    public PostgresCoupleArchiveService(IDbManager dbManager)
    {
        _dbManager = dbManager;
    }

    public async Task<ServiceResult<bool>> MoveCoupleToArchive(int successfulMeetingId)
    {
        var connection = await _dbManager.GetConnectionOrThrow();
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            if (!(await IsMeetingSuccessful(cmd, successfulMeetingId)))
            {
                await transaction.RollbackAsync();
                return ServiceResult<bool>.BadRequest("Під час зустрічі не було вирішено сімейних проблем");
            }
            var coupleIds = await ReadCoupleIds(cmd, successfulMeetingId);
            await InsertCoupleIntoArchive(cmd, coupleIds);
            await transaction.CommitAsync();
            
            return ServiceResult<bool>.Ok(true);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return ServiceResult<bool>.BadRequest(e.Message);
        }
    }

    public async Task<ServiceResult<GetArchivedCoupleResponse>> GetArchivedCouples(GetArchivedCoupleRequest request)
    {
        List<ArchivedCoupleDto> archivedCouples = [];
        var connection = await _dbManager.GetConnectionOrThrow();
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {
            var sql = BuildSqlQueriesForArchivedCouples(request);
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            cmd.CommandText = sql.FullSql;
            var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                archivedCouples.Add(ReadArchivedCouple(reader));
            }

            await reader.CloseAsync();
            cmd.CommandText = $"SELECT COUNT(*) FROM couplearchive {sql.ConditionSql} ";
            var count = (long?)await cmd.ExecuteScalarAsync();

            await transaction.CommitAsync();
            
            return ServiceResult<GetArchivedCoupleResponse>.Ok(new GetArchivedCoupleResponse(archivedCouples, count.Value));
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return ServiceResult<GetArchivedCoupleResponse>.BadRequest(e.Message);
        }
    }

    public async Task<ServiceResult<long>> GetArchivedCouplesCount()
    {
        var connection = await _dbManager.GetConnectionOrThrow();
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            cmd.CommandText = "SELECT COUNT(*) FROM couplearchive";
            var count = (long?)await cmd.ExecuteScalarAsync();
            if (count is null)
            {
                await transaction.RollbackAsync();
                throw new NullReferenceException(nameof(count));
            }

            await transaction.CommitAsync();
            return ServiceResult<long>.Ok(count.Value);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return ServiceResult<long>.BadRequest(e.Message);
        }
    }

    private ArchivedCoupleDto ReadArchivedCouple(DbDataReader reader)
    {
        var coupleArchiveId = reader.GetInt32("couple_archive_id");
        var firstClientId = reader.GetInt32("first_client_id");
        var secondClientId = reader.GetInt32("second_client_id");
        var coupleCreatedOn = DateOnly.FromDateTime(reader.GetDateTime("couple_created_on"));
        var additionalInfo = reader.GetString("additional_info");
        var archivedOn = reader.GetDateTime("archived_on");

        return new ArchivedCoupleDto(coupleArchiveId, firstClientId, secondClientId, coupleCreatedOn, additionalInfo, archivedOn);
    }

    private (string FullSql, string ConditionSql) BuildSqlQueriesForArchivedCouples(GetArchivedCoupleRequest request)
    {
        const string select = "SELECT * FROM couplearchive ";
        const string initialCondition = "WHERE 1=1 ";

        var coupleArchiveIdCondition = request.CoupleArchiveIdFilter.BuildConditionForInteger("couple_archive_id");
        var firstClientIdCondition = request.FirstClientIdFilter.BuildConditionForInteger("first_client_id");
        var secondClientIdCondition = request.SecondClientIdFilter.BuildConditionForInteger("second_client_id");
        var coupleCreatedOnCondition = request.CoupleCreatedOnFilter.BuildConditionForDateOnly("couple_created_on");
        var additionalInfoCondition = request.AddtionalInfoFilter.BuildConditionForString("additional_info");
        var archivedOnCondition = request.ArchivedOnFilter.BuildConditionForDateTime("archived_on");
        var sorting = request.SortingInfo.BuildSortingString();
        var pagination = request.PaginationInfo.ToPostgreSqlPaginationString();

        var conditionSql = string.Concat(initialCondition, coupleArchiveIdCondition, firstClientIdCondition, secondClientIdCondition,
            coupleCreatedOnCondition, additionalInfoCondition, archivedOnCondition);

        var fullSql = string.Concat(select, conditionSql, sorting, pagination);
        return (fullSql, conditionSql);
    }

    private async Task InsertCoupleIntoArchive(DbCommand cmd, (int InviterId, int InviteeId) coupleIds)
    {
        cmd.CommandText =
            "INSERT INTO couplearchive (first_client_id, second_client_id, couple_created_on, additional_info, archived_on) " +
            "VALUES (@firstClientId, @secondClientId, @coupleCreatedOn, @additionalInfo, @archivedOn)";

        cmd.AddParameter("firstClientId", coupleIds.InviterId)
            .AddParameter("secondClientId", coupleIds.InviteeId)
            .AddParameter("coupleCreatedOn", DateOnly.FromDateTime(DateTime.Now))
            .AddParameter("additionalInfo", "")
            .AddParameter("archivedOn", DateTime.Now);

        await cmd.ExecuteNonQueryAsync();
        cmd.Parameters.Clear();
    }

    private static async Task<bool> IsMeetingSuccessful(DbCommand cmd, int successfulMeetingId)
    {
        const string successfulResult = "Підходять";
        cmd.CommandText = "SELECT COUNT(*) FROM meetings WHERE meeting_id = @meetingId AND result = @successfulResult";
        cmd.AddParameter("meetingId", successfulMeetingId)
            .AddParameter("successfulResult", successfulResult);
        var exists = (long?)await cmd.ExecuteScalarAsync();
        cmd.Parameters.Clear();
        return exists == 1;
    }

    private static async Task<(int InviterId, int InviteeId)> ReadCoupleIds(DbCommand cmd, int successfulMeetingId)
    {
        cmd.CommandText = "SELECT inviter_id, invitee_id FROM meetings WHERE meeting_id = @meetingId";
        cmd.AddParameter("meetingId", successfulMeetingId);
        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();
        var inviterId = reader.GetInt32("inviter_id");
        var inviteeId = reader.GetInt32("invitee_id");
        cmd.Parameters.Clear();

        return (inviterId, inviteeId);
    }
}