using System.Data;
using System.Data.Common;
using Common.Filtering.Pagination;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.DTOs.MeetingReviews;
using DatingAgencyMS.Application.DTOs.MeetingReviews.Requests;
using DatingAgencyMS.Application.DTOs.MeetingReviews.Responses;
using DatingAgencyMS.Application.Shared;
using DatingAgencyMS.Infrastructure.Extensions;
using DatingAgencyMS.Infrastructure.Helpers;

namespace DatingAgencyMS.Infrastructure.Services.PostgresServices;

public class PostgresMeetingReviewsService : IMeetingReviewsService
{
    private readonly IDbManager _dbManager;
    
    public PostgresMeetingReviewsService(IDbManager dbManager)
    {
        _dbManager = dbManager;
    }
    
    public async Task<ServiceResult<int>> CreateMeetingReviewAsync(CreateMeetingReviewRequest request)
    {
        var connection = await _dbManager.GetConnectionOrThrow();
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            var meetingExists = await CheckIfMeetingExistsAsync(cmd, request.MeetingId);
            if (!meetingExists)
            {
                await transaction.RollbackAsync();
                return ServiceResult<int>.BadRequest("Зустрічі з вказаним Id не існує");
            }
            
            cmd.CommandText = "INSERT INTO meetingreview (inviter_score, inviter_review, invitee_score, invitee_review, meeting_id) " +
                              "VALUES (@inviterScore, @inviterReview, @inviteeScore, @inviteeReview, @meetingId) RETURNING id";
            cmd.AddParameter("inviterScore", request.InviterScore)
                .AddParameter("inviterReview", request.InviterReview)
                .AddParameter("inviteeScore", request.InviteeScore)
                .AddParameter("inviteeReview", request.InviteeReview)
                .AddParameter("meetingId", request.MeetingId);

            var id = (int?)await cmd.ExecuteScalarAsync();
            await transaction.CommitAsync();
            
            return ServiceResult<int>.Created(id!.Value);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return ServiceResult<int>.ServerError(e.Message);
        }
    }
    private async Task<bool> CheckIfMeetingExistsAsync(DbCommand cmd, int meetingId)
    {
        cmd.CommandText = "SELECT COUNT(*) FROM meetings WHERE meeting_id = @id";
        cmd.AddParameter("id", meetingId);
        var count = (long?)await cmd.ExecuteScalarAsync();
        cmd.Parameters.Clear();
        return count!.Value == 1;
    }
    public async Task<ServiceResult<GetMeetingReviewsResponse>> GetMeetingReviewsAsync(GetMeetingReviewsRequest request)
    {
        List<MeetingReviewDto> dtos = [];
        var connection = await _dbManager.GetConnectionOrThrow();
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {
            var sql = BuildSqlQueries(request);
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            cmd.CommandText = sql.SelectItemsSql;
            var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var dto = ReadMeetingReview(reader);
                dtos.Add(dto);
            }

            await reader.CloseAsync();

            cmd.CommandText = sql.SelectCountSql;
            var count = (long?)await cmd.ExecuteScalarAsync();

            await transaction.CommitAsync();
            
            return ServiceResult<GetMeetingReviewsResponse>.Ok(new GetMeetingReviewsResponse(dtos.ToArray(), count!.Value));
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return ServiceResult<GetMeetingReviewsResponse>.ServerError(e.Message);
        }
    }
    private MeetingReviewDto ReadMeetingReview(DbDataReader reader)
    {
        var id = reader.GetInt32("id");
        var inviterScore = reader.GetInt32("inviter_score");
        var inviterReview = reader.GetString("inviter_review");
        var inviteeScore = reader.GetInt32("invitee_score");
        var inviteeReview = reader.GetString("invitee_review");
        var meetingId = reader.GetInt32("meeting_id");

        return new MeetingReviewDto(id, inviterScore, inviterReview, inviteeScore, inviteeReview, meetingId);
    }
    private (string SelectItemsSql, string SelectCountSql) BuildSqlQueries(GetMeetingReviewsRequest request)
    {
        const string selectFrom = "SELECT * FROM meetingreview ";
        const string initialCondition = "WHERE 1=1 ";
        var idCondition = request.IdFilter.BuildConditionForInteger("id");
        var inviterScoreFilter = request.InviterScoreFilter.BuildConditionForInteger("inviter_score");
        var inviterReviewFilter = request.InviterReviewFilter.BuildConditionForString("inviter_review");
        var inviteeScoreFilter = request.InviteeScoreFilter.BuildConditionForInteger("invitee_score");
        var inviteeReviewFilter = request.InviteeReviewFilter.BuildConditionForString("invitee_review");
        var meetingIdFilter = request.MeetingIdFilter.BuildConditionForInteger("meeting_id");
        var sorting = request.SortingInfo.BuildSortingString();
        var pagination = request.PaginationInfo.ToPostgreSqlPaginationString();

        var conditionSql = string.Concat(initialCondition, idCondition, inviterScoreFilter, inviterReviewFilter, 
            inviteeScoreFilter, inviteeReviewFilter, meetingIdFilter);

        var selectItemsSql = string.Concat(selectFrom, conditionSql, sorting, pagination);
        var selectCountSql = $"SELECT COUNT(*) FROM meetingreview {conditionSql}";

        return (selectItemsSql, selectCountSql);
    }

    public async Task<ServiceResult<MeetingReviewDto>> GetMeetingReviewByIdAsync(int id)
    {
        var connection = await _dbManager.GetConnectionOrThrow();
        await using var transaction = await connection.BeginTransactionAsync();
        try
        {
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            cmd.CommandText = "SELECT * FROM meetingreview WHERE id = @id";
            var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                await reader.CloseAsync();
                await transaction.RollbackAsync();
                return ServiceResult<MeetingReviewDto>.NotFound("Оцінка зустрічі", id);
            }

            var dto = ReadMeetingReview(reader);

            await reader.CloseAsync();
            await transaction.CommitAsync();
            return ServiceResult<MeetingReviewDto>.Ok(dto);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return ServiceResult<MeetingReviewDto>.ServerError(e.Message);
        }
    }
    
    public async Task<ServiceResult<bool>> UpdateMeetingReviewAsync(UpdateMeetingReviewRequest request)
    {
        var connection = await _dbManager.GetConnectionOrThrow();
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();

            cmd.CommandText = "SELECT COUNT(*) FROM meetingreview WHERE id = @id";
            cmd.AddParameter("id", request.Id);

            var count = (long?)await cmd.ExecuteScalarAsync();
            if (count.Value != 1)
            {
                await transaction.RollbackAsync();
                return ServiceResult<bool>.NotFound("Оцінка зустрічі", request.Id);
            }
            
            cmd.Parameters.Clear();
            
            cmd.CommandText = "UPDATE meetingreview " +
                              "SET inviter_score = @inviterScore, " +
                              "inviter_review = @inviterReview, " +
                              "invitee_score = @inviteeScore, " +
                              "invitee_review = @inviteeReview " +
                              "WHERE id = @id";
            cmd.AddParameter("inviterScore", request.InviterScore)
                .AddParameter("inviterReview", request.InviterReview)
                .AddParameter("inviteeScore", request.InviteeScore)
                .AddParameter("inviteeReview", request.InviteeReview)
                .AddParameter("id", request.Id);
            
            await cmd.ExecuteNonQueryAsync();
            await transaction.CommitAsync();
            
            return ServiceResult<bool>.NoContent();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return ServiceResult<bool>.ServerError(e.Message);
        }
    }
    
    public async Task<ServiceResult<bool>> DeleteMeetingReviewAsync(int id)
    {
        var connection = await _dbManager.GetConnectionOrThrow();
        await using var transaction = await connection.BeginTransactionAsync();
        try
        {
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            cmd.CommandText = "SELECT COUNT(*) FROM meetingreview WHERE id = @id";
            cmd.AddParameter("id", id);

            var count = (long?)await cmd.ExecuteScalarAsync();
            if (count!.Value != 1)
            {
                await transaction.RollbackAsync();
                return ServiceResult<bool>.NotFound("Оцінка зустрічі", id);
            }
            
            cmd.Parameters.Clear();

            cmd.CommandText = "DELETE FROM meetingreview WHERE id = @id";
            cmd.AddParameter("id", id);

            await cmd.ExecuteNonQueryAsync();
            await transaction.CommitAsync();
            
            return ServiceResult<bool>.NoContent();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return ServiceResult<bool>.ServerError(e.Message);
        }
    }
}