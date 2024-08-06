using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.DTOs.Meetings.Requests;
using DatingAgencyMS.Application.Shared;
using DatingAgencyMS.Domain.Models.Business;
using DatingAgencyMS.Infrastructure.Extensions;
using DatingAgencyMS.Infrastructure.Helpers;

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
            cmd.AddParameter("date", DateTime.Now)
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
}