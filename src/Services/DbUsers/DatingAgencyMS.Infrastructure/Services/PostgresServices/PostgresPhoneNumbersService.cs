using System.Data;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.DTOs.PhoneNumbers;
using DatingAgencyMS.Application.DTOs.PhoneNumbers.Requests;
using DatingAgencyMS.Application.DTOs.PhoneNumbers.Responses;
using DatingAgencyMS.Application.Shared;
using DatingAgencyMS.Infrastructure.Extensions;
using DatingAgencyMS.Infrastructure.Helpers;

namespace DatingAgencyMS.Infrastructure.Services.PostgresServices;

public class PostgresPhoneNumbersService : IPhoneNumbersService
{
    private readonly IDbManager _dbManager;

    public PostgresPhoneNumbersService(IDbManager dbManager)
    {
        _dbManager = dbManager;
    }

    public async Task<ServiceResult<int>> CreatePhoneNumberAsync(CreatePhoneNumberRequest request)
    {
        var connection = await _dbManager.GetConnectionOrThrow();
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            cmd.CommandText = "INSERT INTO phonenumbers (phone_number, additional_contacts_id) " +
                              "VALUES (@phoneNumber, @additionalContactsId) RETURNING id";
            cmd.AddParameter("phoneNumber", request.PhoneNumber)
                .AddParameter("additionalContactsId", request.AdditionalContactsId);

            var id = (int?)await cmd.ExecuteScalarAsync();
            await transaction.CommitAsync();

            return ServiceResult<int>.Created(id!.Value);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return ServiceResult<int>.BadRequest(e.Message);
        }
    }

    public async Task<ServiceResult<PhoneNumberDto>> GetPhoneNumberByIdAsync(int id)
    {
        var connection = await _dbManager.GetConnectionOrThrow();
        await using var transaction = await connection.BeginTransactionAsync();
        try
        {
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            cmd.CommandText = "SELECT * FROM phonenumbers WHERE id = @id";
            cmd.AddParameter("id", id);

            var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                await transaction.RollbackAsync();
                await reader.CloseAsync();
                return ServiceResult<PhoneNumberDto>.NotFound("Phone Number", id);
            }

            var phoneNumberId = reader.GetInt32("id");
            var phoneNumber = reader.GetString("phone_number");
            var additionalContactsId = reader.GetInt32("additional_contacts_id");

            await transaction.CommitAsync();

            return ServiceResult<PhoneNumberDto>.Ok(new PhoneNumberDto(id, phoneNumber, additionalContactsId));
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return ServiceResult<PhoneNumberDto>.BadRequest(e.Message);
        }
    }

    public async Task<ServiceResult<GetPhoneNumbersResponse>> GetPhoneNumbersAsync(GetPhoneNumbersRequest request)
    {
        List<PhoneNumberDto> dtos = [];
        var connection = await _dbManager.GetConnectionOrThrow();
        await using var transaction = await connection.BeginTransactionAsync();
        try
        {
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            var sql = BuildSqlQuery(request);
            cmd.CommandText = sql.FullSql;
            var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var id = reader.GetInt32("id");
                var phoneNumber = reader.GetString("phone_number");
                var additionalContactsId = reader.GetInt32("additional_contacts_info");
                dtos.Add(new PhoneNumberDto(id, phoneNumber, additionalContactsId));
            }

            await reader.CloseAsync();
            cmd.CommandText = $"SELECT COUNT(*) FROM PhoneNumbers {sql.ConditionalSql}";
            var count = (long?)await cmd.ExecuteScalarAsync();

            await transaction.CommitAsync();
            return ServiceResult<GetPhoneNumbersResponse>.Ok(new GetPhoneNumbersResponse(dtos.ToArray(), count!.Value));
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return ServiceResult<GetPhoneNumbersResponse>.ServerError(e.Message);
        }
    }
    private (string FullSql, string ConditionalSql) BuildSqlQuery(GetPhoneNumbersRequest request)
    {
        var selectFrom = "SELECT * FROM PhoneNumbers ";
        var initialCondition = "WHERE 1=1 ";
        var idCondition = request.IdFilter.BuildConditionForInteger("id");
        var phoneNumberCondition = request.PhoneNumberFilter.BuildConditionForString("phone_number");
        var additionalContactIdCondition = request.AdditionalContactIdFilter.BuildConditionForInteger("additional_contacts_id");
        var sortingString = request.SortingInfo.BuildSortingString();

        var skipItems = ((request.PaginationInfo.PageNumber - 1) * request.PaginationInfo.PageSize);
        var pagination = $"OFFSET {skipItems} ROWS FETCH NEXT {request.PaginationInfo.PageSize} ROWS ONLY";
        var conditionalSql = string.Concat(initialCondition, idCondition, phoneNumberCondition, additionalContactIdCondition);
        return (string.Concat(selectFrom, initialCondition, conditionalSql, sortingString,
                pagination), conditionalSql);
    }

    public async Task<ServiceResult<bool>> UpdatePhoneNumberAsync(UpdatePhoneNumberRequest request)
    {
        var connection = await _dbManager.GetConnectionOrThrow();
        await using var transaction = await connection.BeginTransactionAsync();
        try
        {
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            cmd.CommandText = "SELECT COUNT(*) FROM PhoneNumbers WHERE id = @id";
            cmd.AddParameter("id", request.Id);
            var matchingRows = (long?)await cmd.ExecuteScalarAsync();
            if (matchingRows!.Value != 1)
            {
                await transaction.RollbackAsync();
                return ServiceResult<bool>.NotFound("PhoneNumber", request.Id);
            }
            
            cmd.Parameters.Clear();
            cmd.CommandText = "UPDATE PhoneNumbers " +
                              "SET phone_number = @phoneNumber " +
                              "WHERE id = @id";
            cmd.AddParameter("phoneNumber", request.PhoneNumber);
            cmd.AddParameter("id", request.Id);

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

    public async Task<ServiceResult<bool>> DeletePhoneNumberAsync(int id)
    {
        var connection = await _dbManager.GetConnectionOrThrow();
        await using var transaction = await connection.BeginTransactionAsync();
        try
        {
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            cmd.CommandText = "DELETE FROM PhoneNumbers WHERE id = @id";
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