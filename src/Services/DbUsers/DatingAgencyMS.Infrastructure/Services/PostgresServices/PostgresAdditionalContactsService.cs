using System.Collections.Immutable;
using System.Data;
using System.Data.Common;
using Common.Filtering.Pagination;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.DTOs.AdditionalContacts;
using DatingAgencyMS.Application.DTOs.AdditionalContacts.Requests;
using DatingAgencyMS.Application.DTOs.AdditionalContacts.Responses;
using DatingAgencyMS.Application.Shared;
using DatingAgencyMS.Infrastructure.Extensions;
using DatingAgencyMS.Infrastructure.Helpers;

namespace DatingAgencyMS.Infrastructure.Services.PostgresServices;

public class PostgresAdditionalContactsService : IAdditionalContactsService
{
    private readonly IDbManager _dbManager;

    public PostgresAdditionalContactsService(IDbManager dbManager)
    {
        _dbManager = dbManager;
    }

    public async Task<ServiceResult<GetAdditionalContactsResponse>> GetAsync(GetAdditionalContactsRequest request)
    {
        List<AdditionalContactDto> additionalContactDtos = [];
        var connection = await _dbManager.GetConnectionOrThrow();
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {
            var sql = BuildSqlQueries(request);
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            cmd.CommandText = sql.FullSql;

            var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                additionalContactDtos.Add(ReadAdditionalContact(reader));
            }

            await reader.CloseAsync();

            cmd.CommandText = $"SELECT COUNT(*) FROM additionalcontacts {sql.ConditionSql}";
            var count = (long?)await cmd.ExecuteScalarAsync();

            await transaction.CommitAsync();
            
            return ServiceResult<GetAdditionalContactsResponse>.Ok(new GetAdditionalContactsResponse(additionalContactDtos, count.Value));
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return ServiceResult<GetAdditionalContactsResponse>.BadRequest(e.Message);
        }
    }

    public async Task<ServiceResult<bool>> CreateAsync(CreateAdditionalContactsRequest request)
    {
        var connection = await _dbManager.GetConnectionOrThrow();
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            cmd.CommandText = "SELECT COUNT(*) FROM additionalcontacts WHERE client_id = @clientId";
            cmd.AddParameter("clientId", request.ClientId);
            var count = (long?)await cmd.ExecuteScalarAsync();

            if (count == 1)
            {
                await transaction.RollbackAsync();
                return ServiceResult<bool>.BadRequest("Об'єкт додаткових записів для даного користувача вже існує.");
            }
            cmd.Parameters.Clear();

            cmd.CommandText = "SELECT COUNT(*) FROM clients WHERE id = @clientId";
            cmd.AddParameter("clientId", request.ClientId);
            count = (long?)await cmd.ExecuteScalarAsync();
            if (count != 1)
            {
                await transaction.RollbackAsync();
                return ServiceResult<bool>.NotFound("Клієнт", request.ClientId);
            }
            cmd.Parameters.Clear();
            
            cmd.CommandText = "INSERT INTO additionalcontacts (client_id, telegram, facebook, instagram, tiktok) " +
                              "VALUES (@clientId, @telegram, @facebook, @instagram, @tiktok)";
            cmd.AddParameter("clientId", request.ClientId)
                .AddParameter("telegram", request.Telegram)
                .AddParameter("facebook", request.Facebook)
                .AddParameter("instagram", request.Instagram)
                .AddParameter("tiktok", request.TikTok);

            var rowsAffected = await cmd.ExecuteNonQueryAsync();

            await transaction.CommitAsync();

            return rowsAffected == 1
                ? ServiceResult<bool>.Created(true)
                : ServiceResult<bool>.BadRequest("Помилка при створенні запису");
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return ServiceResult<bool>.BadRequest(e.Message);
        }
    }

    public async Task<ServiceResult<bool>> UpdateAsync(UpdateAdditionalContactsRequest request)
    {
        var connection = await _dbManager.GetConnectionOrThrow();
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            cmd.CommandText = "UPDATE additionalcontacts " +
                              "SET facebook = @facebook, " +
                              "instagram = @instagram, " +
                              "telegram = @telegram, " +
                              "tiktok = @tiktok " +
                              "WHERE id = @id";
            cmd.AddParameter("facebook", request.Facebook)
                .AddParameter("instagram", request.Instagram)
                .AddParameter("telegram", request.Telegram)
                .AddParameter("tiktok", request.TikTok)
                .AddParameter("id", request.Id);

            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            await transaction.CommitAsync();

            return rowsAffected == 1
                ? ServiceResult<bool>.Ok(true)
                : ServiceResult<bool>.BadRequest("Помилка при оновлені об'єкта додаткових контактів");
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return ServiceResult<bool>.BadRequest(e.Message);
        }
    }

    public async Task<ServiceResult<GetAdditionalContactByIdResponse>> GetByIdAsync(int id)
    {
        var connection = await _dbManager.GetConnectionOrThrow();
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            cmd.CommandText = "SELECT * FROM additionalcontacts WHERE id = @id";
            cmd.AddParameter("id", id);

            var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                await reader.CloseAsync();
                await transaction.RollbackAsync();
                return ServiceResult<GetAdditionalContactByIdResponse>.NotFound("Додаткові контакти", id);
            }

            var additionalContacts = ReadAdditionalContact(reader);

            await reader.CloseAsync();
            await transaction.CommitAsync();
            
            return ServiceResult<GetAdditionalContactByIdResponse>.Ok(new GetAdditionalContactByIdResponse(additionalContacts));
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return ServiceResult<GetAdditionalContactByIdResponse>.BadRequest(e.Message);
        }
    }

    private AdditionalContactDto ReadAdditionalContact(DbDataReader reader)
    {
        var id = reader.GetInt32("id");
        var clientId = reader.GetInt32("client_id");
        var telegram = reader.GetString("telegram");
        var facebook = reader.GetString("facebook");
        var instagram = reader.GetString("instagram");
        var tiktok = reader.GetString("tiktok");

        return new AdditionalContactDto(id, clientId, telegram, facebook, instagram, tiktok);
    }

    private (string FullSql, string ConditionSql) BuildSqlQueries(GetAdditionalContactsRequest request)
    {
        const string select = "SELECT * FROM additionalcontacts ";
        const string initialCondition = "WHERE 1=1 ";
        var idCondition = request.IdFilter.BuildConditionForInteger("id");
        var clientIdCondition = request.ClientIdFilter.BuildConditionForInteger("client_id");
        var telegramCondition = request.TelegramFilter.BuildConditionForString("telegram");
        var facebookCondition = request.FacebookFilter.BuildConditionForString("facebook");
        var instagramCondition = request.InstagramFilter.BuildConditionForString("instagram");
        var tiktokCondition = request.TikTokFilter.BuildConditionForString("tiktok");
        var sorting = request.SortingInfo.BuildSortingString();
        var pagination = request.PaginationInfo.ToPostgreSqlPaginationString();

        var conditionSql = string.Concat(initialCondition, idCondition, clientIdCondition,
            telegramCondition, facebookCondition, instagramCondition, tiktokCondition);

        var fullSql = string.Concat(select, conditionSql, sorting, pagination);
        return (fullSql, conditionSql);
    }
}