using System.Data.Common;
using Common.Filtering.Pagination;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.DTOs.Meetings;
using DatingAgencyMS.Application.DTOs.Meetings.Requests;
using DatingAgencyMS.Application.DTOs.Meetings.Responses;
using DatingAgencyMS.Application.Shared;
using DatingAgencyMS.Domain.Models.Business;
using DatingAgencyMS.Infrastructure.Extensions;
using DatingAgencyMS.Infrastructure.Helpers;
using System.Data;


namespace DatingAgencyMS.Infrastructure.Services.PostgresServices;

public class PostgresMeetingsService : IMeetingsService
{
    private readonly IDbManager _dbManager;

    public PostgresMeetingsService(IDbManager dbManager)
    {
        _dbManager = dbManager;
    }

    public async Task<ServiceResult<int>> CreateMeeting(CreateMeetingRequest request)
    {
        var connection = await _dbManager.GetConnectionOrThrow();
        await using var transaction = await connection.BeginTransactionAsync();
        try
        {
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            cmd.CommandText = "INSERT INTO meetings (date, inviter_id, invitee_id, location, result) " +
                              "VALUES (@date, @inviterId, @inviteeId, @location, @result) RETURNING meeting_id";
            cmd.AddParameter("date", request.Date)
                .AddParameter("inviterId", request.InviterId)
                .AddParameter("inviteeId", request.InviteeId)
                .AddParameter("location", request.Location)
                .AddParameter("result", MeetingResultHelper.ToUkrainian(MeetingResult.Pending));

            var meetingId = await cmd.ExecuteNonQueryAsync();
            await transaction.CommitAsync();
            return ServiceResult<int>.Ok(meetingId);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return ServiceResult<int>.BadRequest(e.Message);
        }
    }

    public async Task<ServiceResult<GetMeetingsResponse>> GetMeetings(GetMeetingsRequest request)
    {
        var connection = await _dbManager.GetConnectionOrThrow();
        await using var transaction = await connection.BeginTransactionAsync();
        try
        {
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            var sql = BuildSqlQueryForMeetings(request);

            cmd.CommandText = sql.FullSql;
            var reader = await cmd.ExecuteReaderAsync();
            var meetings = await ReadMeetings(reader);
            await reader.CloseAsync();

            cmd.CommandText = $"SELECT COUNT(*) FROM meetings {sql.ConditionSql}";
            var count = (long?)await cmd.ExecuteScalarAsync();
            await transaction.CommitAsync();

            return ServiceResult<GetMeetingsResponse>.Ok(new GetMeetingsResponse(meetings, count.Value));
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return ServiceResult<GetMeetingsResponse>.BadRequest(e.Message);
        }
    }

    public async Task<ServiceResult<GetMeetingsResponse>> GetPlannedMeetingsByPeriod(GetPlannedMeetingsForPeriodRequest request)
    {
        var connection = await _dbManager.GetConnectionOrThrow();
        await using var transaction = await connection.BeginTransactionAsync();
        try
        {
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            cmd.CommandText = "SELECT * FROM meetings WHERE result = 'Очікується' " +
                              "AND EXTRACT(YEAR FROM date) = @year " +
                              "AND EXTRACT(MONTH FROM date) = @month " +
                              "ORDER BY date " +
                              "OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY ";
            cmd.AddParameter("year", request.Year)
                .AddParameter("month", request.Month)
                .AddParameter("skip", (request.PageNumber - 1) * request.PageSize)
                .AddParameter("take", request.PageSize);

            var reader = await cmd.ExecuteReaderAsync();
            var meetings = await ReadMeetings(reader);
            await reader.CloseAsync();
            
            cmd.Parameters.Clear();
            cmd.CommandText = "SELECT COUNT(*) FROM meetings WHERE result = 'Очікується' " +
                              "AND EXTRACT(YEAR FROM date) = @year " +
                              "AND EXTRACT(MONTH FROM date) = @month ";
            cmd.AddParameter("year", request.Year)
                .AddParameter("month", request.Month);
            var count = (long?)await cmd.ExecuteScalarAsync();
            
            return ServiceResult<GetMeetingsResponse>.Ok(new GetMeetingsResponse(meetings, count.Value));
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return ServiceResult<GetMeetingsResponse>.BadRequest(e.Message);
        }
    }

    private async Task<IReadOnlyList<MeetingDto>> ReadMeetings(DbDataReader reader)
    {
        List<MeetingDto> meetings = [];
        while (await reader.ReadAsync())
        {
            var meetingId = reader.GetInt32("meeting_id");
            var date = reader.GetDateTime("date");
            var inviterId = reader.GetInt32("inviter_id");
            var inviteeId = reader.GetInt32("invitee_id");
            var location = reader.GetString("location");
            var result = MeetingResultHelper.FromUkrainianToEnum(reader.GetString("result"));
            
            meetings.Add(new MeetingDto(meetingId, date, inviterId, inviteeId, location, result));
        }

        return meetings;
    }

    private (string FullSql, string ConditionSql) BuildSqlQueryForMeetings(GetMeetingsRequest request)
    {
        var select = "SELECT * FROM meetings ";
        var initialCondition = "WHERE 1=1 ";

        var meetingIdCondition = request.MeetingIdFilter.BuildConditionForInteger("meeting_id");
        var dateCondition = request.DateFilter.BuildConditionForDateTime("date");
        var inviterIdCondition = request.InviterIdFilter.BuildConditionForInteger("inviter_id");
        var inviteeIdConditon = request.InviteeIdFilter.BuildConditionForInteger("invitee_id");
        var locationCondition = request.LocationFilter.BuildConditionForString("location");
        var sorting = request.SortingInfo.BuildSortingString();
        var pagination = request.PaginationInfo.ToPostgreSqlPaginationString();

        var conditionSql = string.Concat(initialCondition, meetingIdCondition, dateCondition, inviterIdCondition, inviteeIdConditon,
            locationCondition);
        var fullSql = string.Concat(select, conditionSql, sorting, pagination);

        return (fullSql, conditionSql);
    }
}