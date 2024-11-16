using System.Data;
using System.Data.Common;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.DTOs.Invitations;
using DatingAgencyMS.Application.DTOs.Invitations.Requests;
using DatingAgencyMS.Application.DTOs.Invitations.Response;
using DatingAgencyMS.Application.Shared;
using DatingAgencyMS.Infrastructure.Extensions;
using DatingAgencyMS.Infrastructure.Helpers;

namespace DatingAgencyMS.Infrastructure.Services.PostgresServices;

public class PostgresInvitationsService : IInvitationsService
{
    private readonly IDbManager _dbManager;

    public PostgresInvitationsService(IDbManager dbManager)
    {
        _dbManager = dbManager;
    }

    public async Task<ServiceResult<GetInvitationsResponse>> GetInvitations(GetInvitationsRequest request)
    {
        List<InvitationDto> invitations = [];
        var connection = await _dbManager.GetConnectionOrThrow();
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {
            var sql = BuildSqlForGetInvitations(request);
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            cmd.CommandText = sql.FullSql;
            
            var reader = await cmd.ExecuteReaderAsync();
            await ReadInvitations(reader, invitations);
            await reader.CloseAsync();

            cmd.CommandText = $"SELECT COUNT(*) FROM invitations {sql.ConditionSql}";
            var count = (long?)await cmd.ExecuteScalarAsync();
            
            await transaction.CommitAsync();
            return ServiceResult<GetInvitationsResponse>.Ok(new GetInvitationsResponse(invitations, count.Value));
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return ServiceResult<GetInvitationsResponse>.BadRequest(e.Message);
        }
    }

    public async Task<ServiceResult<int>> CreateInvitation(CreateInvitationRequest request)
    {
        var connection = await _dbManager.GetConnectionOrThrow();
        await using var transaction = await connection.BeginTransactionAsync();
        try
        {
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            cmd.CommandText = "SELECT COUNT(*) FROM clients WHERE id = @id";
            cmd.AddParameter("id", request.InviterId);
            var count = (long?)await cmd.ExecuteScalarAsync();
            if (count != 1)
            {
                await transaction.RollbackAsync();
                return ServiceResult<int>.NotFound("Клієнт", request.InviterId);
            }
            cmd.Parameters.Clear();
            
            cmd.CommandText = "SELECT COUNT(*) FROM clients WHERE id = @id";
            cmd.AddParameter("id", request.InviteeId);
            count = (long?)await cmd.ExecuteScalarAsync();
            if (count != 1)
            {
                await transaction.RollbackAsync();
                return ServiceResult<int>.NotFound("Клієнт", request.InviteeId);
            }
            cmd.Parameters.Clear();
            
            cmd.CommandText =
                "INSERT INTO invitations (inviter_id, invitee_id, location, date_of_meeting, created_on, active_to, is_accepted) " +
                "VALUES (@inviterId, @inviteeId, @location, @dateOfMeeting, @createdOn, @activeTo, @isAccepted) RETURNING invitation_id";

            var createdOn = DateOnly.FromDateTime(DateTime.Now);
            var activeTo = createdOn.AddDays(7);

            cmd.AddParameter("inviterId", request.InviterId)
                .AddParameter("inviteeId", request.InviteeId)
                .AddParameter("location", request.Location)
                .AddParameter("dateOfMeeting", request.DateOfMeeting)
                .AddParameter("createdOn", createdOn)
                .AddParameter("activeTo", activeTo)
                .AddParameter("isAccepted", false);

            var id = await cmd.ExecuteNonQueryAsync();
            await transaction.CommitAsync();
            
            return ServiceResult<int>.Ok(id);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return ServiceResult<int>.BadRequest(e.Message);
        }
    }

    public async Task<ServiceResult<bool>> DeleteInvitation(int invitationId, string requestedBy)
    {
        var connection = await _dbManager.GetConnectionOrThrow();
        await using var transaction = await connection.BeginTransactionAsync();
        try
        {
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            cmd.CommandText = "DELETE FROM invitations WHERE invitation_id = @invitationId";
            cmd.AddParameter("invitationId", invitationId);
            
            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            if (rowsAffected == 0)
            {
                await transaction.RollbackAsync();
                return ServiceResult<bool>.NotFound("Запрошення", invitationId);
            }
            
            await transaction.CommitAsync();
            return ServiceResult<bool>.NoContent();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return ServiceResult<bool>.BadRequest(e.Message);
        }
    }

    public async Task<ServiceResult<InvitationDto>> MarkAsAccepted(int invitationId)
    {
        var connection = await _dbManager.GetConnectionOrThrow();
        await using var transaction = await connection.BeginTransactionAsync();
        try
        {
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            cmd.CommandText = "UPDATE invitations SET is_accepted = TRUE WHERE invitation_id = @invitationId";
            cmd.AddParameter("invitationId", invitationId);
            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            if (rowsAffected == 0)
            {
                await transaction.RollbackAsync();
                return ServiceResult<InvitationDto>.BadRequest("Запрошення вже прийнято або його не було знайдено");
            }

            cmd.CommandText = "SELECT * FROM invitations WHERE invitation_id = @invitationId";
            cmd.AddParameter("invitationId", invitationId);
            var reader = await cmd.ExecuteReaderAsync();
            await reader.ReadAsync();
            var invitation = ReadSingleInvitation(reader);
            await reader.CloseAsync();
            await transaction.CommitAsync();
            return ServiceResult<InvitationDto>.Ok(invitation);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return ServiceResult<InvitationDto>.BadRequest(e.Message);
        }
    }

    private static async Task ReadInvitations(DbDataReader reader, List<InvitationDto> invitations)
    {
        while (await reader.ReadAsync())
        {
            var invitation = ReadSingleInvitation(reader);
            invitations.Add(invitation);
        }
    }

    private static InvitationDto ReadSingleInvitation(DbDataReader reader)
    {
        var invitationId = reader.GetInt32("invitation_id");
        var inviterId = reader.GetInt32("inviter_id");
        var inviteeId = reader.GetInt32("invitee_id");
        var location = reader.GetString("location");
        var dateOfMeeting = reader.GetDateTime("date_of_meeting");
        var createdOn = DateOnly.FromDateTime(reader.GetDateTime("created_on"));
        var activeTo = DateOnly.FromDateTime(reader.GetDateTime("active_to"));
        var isAccepted = reader.GetBoolean("is_accepted");

        return new InvitationDto(invitationId, inviterId, inviteeId,
            location, dateOfMeeting, createdOn, activeTo, isAccepted);
    }

    private static (string FullSql, string ConditionSql) BuildSqlForGetInvitations(GetInvitationsRequest request)
    {
        var select = "SELECT * FROM invitations ";
        var initialCondition = "WHERE 1=1 ";
        var invitationIdCondition = request.InvitationIdFilter.BuildConditionForInteger("invitation_id");
        var inviterIdCondition = request.InviterIdFilter.BuildConditionForInteger("inviter_id");
        var inviteeIdCondition = request.InviteeIdFilter.BuildConditionForInteger("invitee_id");
        var locationCondition = request.LocationFilter.BuildConditionForString("location");
        var dateOfMeetingCondition = request.DateOfMeetingFilter.BuildConditionForDateTime("date_of_meeting");
        var createdOnCondition = request.CreatedOnFilter.BuildConditionForDateOnly("created_on");
        var activeToCondition = request.ActiveToFilter.BuildConditionForDateOnly("active_to");
        var isAcceptedCondition = request.IsAcceptedFilter.BuildConditionForBoolean("is_accepted");
        var skip = (request.PaginationInfo.PageNumber - 1) * request.PaginationInfo.PageSize;
        var take = request.PaginationInfo.PageSize;
        var sortingInfo = request.SortingInfo.BuildSortingString();
        var paging = $"OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";
        var conditionSql = string.Concat(initialCondition, invitationIdCondition, inviterIdCondition, inviteeIdCondition,
            locationCondition, dateOfMeetingCondition, createdOnCondition, activeToCondition, isAcceptedCondition);
        
        return (string.Concat(select, conditionSql, sortingInfo, paging), conditionSql);
    }
}