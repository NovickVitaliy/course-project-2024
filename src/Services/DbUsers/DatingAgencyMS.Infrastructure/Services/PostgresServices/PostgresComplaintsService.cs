using System.Data;
using Common.Filtering.Pagination;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.DTOs.Complaints;
using DatingAgencyMS.Application.DTOs.Complaints.Requests;
using DatingAgencyMS.Application.DTOs.Complaints.Responses;
using DatingAgencyMS.Application.Shared;
using DatingAgencyMS.Domain.Models.Business;
using DatingAgencyMS.Infrastructure.Extensions;
using DatingAgencyMS.Infrastructure.Helpers;

namespace DatingAgencyMS.Infrastructure.Services.PostgresServices;

public class PostgresComplaintsService : IComplaintsService
{
    private readonly IDbManager _dbManager;
    
    public PostgresComplaintsService(IDbManager dbManager)
    {
        _dbManager = dbManager;
    }
    
    public async Task<ServiceResult<int>> CreateComplaintAsync(CreateComplaintRequest request)
    {
        var connection = await _dbManager.GetConnectionOrThrow();
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            cmd.CommandText = "INSERT INTO complaints(complainant_id, complainee_id, date, text, complaint_status) " +
                              "VALUES (@complainantIt, @complaineeId, @date, @text, @complaintStatus) RETURNING complaint_id";
            cmd.AddParameter("complainantId", request.ComplainantId)
                .AddParameter("complaineeId", request.ComplaineeId)
                .AddParameter("date", DateTime.UtcNow)
                .AddParameter("text", request.Text)
                .AddParameter("complaintStatus", ComplaintStatus.Pending.ToUkrainian());

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
    
    public async Task<ServiceResult<GetComplaintsResponse>> GetComplaintsAsync(GetComplaintsRequest request)
    {
        List<ComplaintDto> dtos = [];
        var connection = await _dbManager.GetConnectionOrThrow();
        await using var transaction = await connection.BeginTransactionAsync();
        try
        {
            var sql = BuildSqlQueries(request);
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            cmd.CommandText = sql.FullSql;
            var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var complaintId = reader.GetInt32("complaint_id");
                var complainantId = reader.GetInt32("complainant_id");
                var complaineeId = reader.GetInt32("complainee_id");
                var date = reader.GetDateTime("date");
                var text = reader.GetString("text");
                var complainantStatus = ComplaintStatusTranslator.ToEnum(reader.GetString("complaint_status"));
                
                dtos.Add(new ComplaintDto(complaintId, complainantId, complaineeId, date, text, complainantStatus));
            }

            await reader.CloseAsync();

            cmd.CommandText = $"SELECT COUNT(*) FROM complaints {sql.ConditionSql}";
            var count = (long?)await cmd.ExecuteScalarAsync();

            await transaction.CommitAsync();
            
            return ServiceResult<GetComplaintsResponse>.Ok(new GetComplaintsResponse(dtos.ToArray(), count!.Value));
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return ServiceResult<GetComplaintsResponse>.ServerError(e.Message);
        }
    }
    private (string FullSql, string ConditionSql) BuildSqlQueries(GetComplaintsRequest request)
    {
        const string selectFrom = "SELECT * FROM complaints ";
        const string initialCondition = "WHERE 1=1 ";
        var complaintIdCondition = request.ComplaintIdFilter.BuildConditionForInteger("complaint_id");
        var complainantIdCondition = request.ComplainantIdFilter.BuildConditionForInteger("complainant_id");
        var complaineeIdCondition = request.ComplaineeIdFilter.BuildConditionForInteger("complainee_id");
        var dateCondition = request.DateFilter.BuildConditionForDateTime("date");
        var textCondition = request.TextFilter.BuildConditionForString("text");
        var complaintStatusFilter = request.ComplaintStatusFilter.BuildConditionForString("complaint_status");
        var sortingString = request.SortingInfo.BuildSortingString();
        var pagination = request.PaginationInfo.ToPostgreSqlPaginationString();

        var conditionSql = string.Concat(initialCondition, complaintIdCondition, complainantIdCondition, complaineeIdCondition,
            dateCondition, textCondition, complaintStatusFilter);

        return (string.Concat(selectFrom, conditionSql, sortingString, pagination), conditionSql);
    }

    public async Task<ServiceResult<ComplaintDto>> GetComplaintByIdAsync(int id)
    {
        var connection = await _dbManager.GetConnectionOrThrow();
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            cmd.CommandText = "SELECT * FROM complaints WHERE complaint_id = @complaintId";
            cmd.AddParameter("complaintId", id);

            var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                await reader.CloseAsync();
                await transaction.RollbackAsync();
                return ServiceResult<ComplaintDto>.NotFound("Скарга", id);
            }
            
            var complaintId = reader.GetInt32("complaint_id");
            var complainantId = reader.GetInt32("complainant_id");
            var complaineeId = reader.GetInt32("complainee_id");
            var date = reader.GetDateTime("date");
            var text = reader.GetString("text");
            var complainantStatus = ComplaintStatusTranslator.ToEnum(reader.GetString("complaint_status"));

            await reader.CloseAsync();
            await transaction.CommitAsync();
            
            return ServiceResult<ComplaintDto>.Ok(new ComplaintDto(complaintId, complainantId, complaineeId, date, text, complainantStatus));
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return ServiceResult<ComplaintDto>.ServerError(e.Message);
        }
    }
    
    public async Task<ServiceResult<bool>> UpdateComplaintAsync(UpdateComplaintRequest request)
    {
        var connection = await _dbManager.GetConnectionOrThrow();
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            cmd.CommandText = "SELECT COUNT(*) FROM complaints WHERE complaint_id = @complaintId";
            cmd.AddParameter("complaintId", request.ComplaintId);
            var count = (long?)await cmd.ExecuteScalarAsync();
            if (count!.Value != 1)
            {
                await transaction.RollbackAsync();
                return ServiceResult<bool>.NotFound("Скарга", request.ComplaintId);
            }
            
            cmd.Parameters.Clear();
            cmd.CommandText = "UPDATE complaints " +
                              "SET text = @text " +
                              "WHERE complaint_id = @complaintId";
            cmd.AddParameter("text", request.Text)
                .AddParameter("complaintId", request.ComplaintId);
            
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
    public async Task<ServiceResult<bool>> DeleteComplaintAsync(int id)
    {
        var connection = await _dbManager.GetConnectionOrThrow();
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            cmd.CommandText = "SELECT COUNT(*) FROM complaints WHERE complaint_id = @complaintId";
            cmd.AddParameter("complaintId", id);

            var count = (long?)await cmd.ExecuteScalarAsync();
            if (count!.Value != 1)
            {
                await transaction.RollbackAsync();
                return ServiceResult<bool>.NotFound("Скарга", id);
            }
            
            cmd.Parameters.Clear();
            cmd.CommandText = "DELETE FROM complaints WHERE complaint_id = @complaintId";
            cmd.AddParameter("complaintId", id);

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