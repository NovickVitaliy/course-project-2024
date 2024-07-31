using System.Data;
using System.Data.Common;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.DTOs.PartnerRequirements;
using DatingAgencyMS.Application.DTOs.PartnerRequirements.Requests;
using DatingAgencyMS.Application.DTOs.PartnerRequirements.Responses;
using DatingAgencyMS.Application.Shared;
using DatingAgencyMS.Domain.Models.Business;
using DatingAgencyMS.Infrastructure.Extensions;
using DatingAgencyMS.Infrastructure.Helpers;

namespace DatingAgencyMS.Infrastructure.Services;

public class PostgresPartnerRequirementsService : IPartnerRequirementsService
{
    private readonly IDbManager _dbManager;

    public PostgresPartnerRequirementsService(IDbManager dbManager)
    {
        _dbManager = dbManager;
    }

    public async Task<ServiceResult<bool>> CreatePartnerRequirements(CreatePartnerRequirementsRequest request)
    {
        var connection = await GetConnection(request.RequestedBy);
        await using var transaction = await connection.BeginTransactionAsync();
        try
        {
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            cmd.CommandText = "INSERT INTO partnerrequirements (gender, sex, min_age, max_age, min_height, max_height, min_weight, max_weight, zodiac_sign, location, client_id) " +
                              "VALUES (@gender, @sex, @minAge, @maxAge, @minHeight, @maxHeight, @minWeight, @maxWeight, @zodiacSign, @location, @clientId)";
            cmd.AddParameter("gender", request.Gender);
            cmd.AddParameter("sex", request.Sex);
            cmd.AddParameter("minAge", request.MinAge);
            cmd.AddParameter("maxAge", request.MaxAge);
            cmd.AddParameter("minHeight", request.MinHeight);
            cmd.AddParameter("maxHeight", request.MaxHeight);
            cmd.AddParameter("minWeight", request.MinWeight);
            cmd.AddParameter("maxWeight", request.MaxWeight);
            cmd.AddParameter("zodiacSign", request.ZodiacSign is null ? null : ZodiacSignHelper.GetUkrainianTranslation(request.ZodiacSign.Value));
            cmd.AddParameter("location", request.City);
            cmd.AddParameter("clientId", request.ClientId);

            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            await transaction.CommitAsync();

            if (rowsAffected == 0)
            {
                return ServiceResult<bool>.BadRequest("Не вдалося добавити об'єкт вимог до партнера до БД");
            }
            
            return ServiceResult<bool>.Created(true);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return ServiceResult<bool>.BadRequest(e.Message);
        }
    }

    public async Task<ServiceResult<GetPartnersRequirementResponse>> GetPartnersRequirement(GetPartnersRequirementRequest request)
    {
        var connection = await GetConnection(request.RequestedBy);
        await using var transaction = await connection.BeginTransactionAsync();
        try
        {
            List<PartnerRequirementsDto> partnerRequirementsDtos = [];
            var sql = BuildSqlQueries(request);
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            cmd.CommandText = sql.FullSqlQuery;
            var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var id = reader.GetInt32("requirement_id");
                var gender = await reader.IsDBNullAsync("gender") ? null : reader.GetString("gender");
                var sex = await reader.IsDBNullAsync("sex") ? null : reader.GetString("sex");
                int? minAge = await reader.IsDBNullAsync("min_age") ? null : reader.GetInt32("min_age");
                int? maxAge = await reader.IsDBNullAsync("max_age") ? null : reader.GetInt32("max_age");
                int? minHeight = await reader.IsDBNullAsync("min_height") ? null : reader.GetInt32("min_height");
                int? maxHeight = await reader.IsDBNullAsync("max_height") ? null : reader.GetInt32("max_height");
                int? minWeight = await reader.IsDBNullAsync("min_weight") ? null : reader.GetInt32("min_weight");
                int? maxWeight = await reader.IsDBNullAsync("max_weight") ? null : reader.GetInt32("max_weight");
                var ukrainianZodiac = await reader.IsDBNullAsync("zodiac_sign") ? null : reader.GetString("zodiac_sign");
                ZodiacSign? zodiacSign = ukrainianZodiac is not null
                    ? ZodiacSignHelper.FromUkrainianToZodiacSign(ukrainianZodiac)
                    : null;
                var location = await reader.IsDBNullAsync("location") ? null :reader.GetString("location");
                var clientId = reader.GetInt32("client_id");
                partnerRequirementsDtos.Add(new PartnerRequirementsDto(id, gender, sex, minAge, maxAge, minHeight, 
                    maxHeight, minWeight, maxWeight, zodiacSign, location, clientId));
            }

            await reader.CloseAsync();
            cmd.CommandText = $"SELECT COUNT(*) FROM partnerrequirements {sql.ConditionOnlySqlQuery}";
            var count = (long?)await cmd.ExecuteScalarAsync();

            await transaction.CommitAsync();
            return ServiceResult<GetPartnersRequirementResponse>.Ok(new GetPartnersRequirementResponse(partnerRequirementsDtos, count.Value));
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return ServiceResult<GetPartnersRequirementResponse>.BadRequest(e.Message);
        }
    }

    public async Task<ServiceResult<GetPartnerRequirementResponse>> GetPartnerRequirementById(int id, string requestedBy)
    {
        var connection = await GetConnection(requestedBy);
        await using var transaction = await connection.BeginTransactionAsync();
        try
        {
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            cmd.CommandText = "SELECT * FROM partnerrequirements WHERE requirement_id = @id";
            cmd.AddParameter("id", id);
            
            var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                await reader.CloseAsync();
                await transaction.CommitAsync();
                return ServiceResult<GetPartnerRequirementResponse>.NotFound("Вимоги до партнера", id);
            }
            
            var gender = await reader.IsDBNullAsync("gender") ? null : reader.GetString("gender");
            var sex = await reader.IsDBNullAsync("sex") ? null : reader.GetString("sex");
            int? minAge = await reader.IsDBNullAsync("min_age") ? null : reader.GetInt32("min_age");
            int? maxAge = await reader.IsDBNullAsync("max_age") ? null : reader.GetInt32("max_age");
            int? minHeight = await reader.IsDBNullAsync("min_height") ? null : reader.GetInt32("min_height");
            int? maxHeight = await reader.IsDBNullAsync("max_height") ? null : reader.GetInt32("max_height");
            int? minWeight = await reader.IsDBNullAsync("min_weight") ? null : reader.GetInt32("min_weight");
            int? maxWeight = await reader.IsDBNullAsync("max_weight") ? null : reader.GetInt32("max_weight");
            var ukrainianZodiac = await reader.IsDBNullAsync("zodiac_sign") ? null : reader.GetString("zodiac_sign");
            ZodiacSign? zodiacSign = ukrainianZodiac is not null
                ? ZodiacSignHelper.FromUkrainianToZodiacSign(ukrainianZodiac)
                : null;
            var location = await reader.IsDBNullAsync("location") ? null :reader.GetString("location");
            var clientId = reader.GetInt32("client_id");
            
            var partnerRequirementsDto = new PartnerRequirementsDto(id, gender, sex, minAge, maxAge, minHeight, 
                maxHeight, minWeight, maxWeight, zodiacSign, location, clientId);

            await reader.CloseAsync();
            await transaction.CommitAsync();

            return ServiceResult<GetPartnerRequirementResponse>.Ok(new GetPartnerRequirementResponse(partnerRequirementsDto));
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return ServiceResult<GetPartnerRequirementResponse>.BadRequest(e.Message);
        }
    }

    public async Task<ServiceResult<bool>> UpdatePartnerRequirement(int partnerRequirementId, UpdatePartnerRequirementRequest request)
    {
        var connection = await GetConnection(request.RequestedBy);
        await using var transaction = await connection.BeginTransactionAsync();
        try
        {
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            cmd.CommandText = "UPDATE partnerrequirements SET " +
                              "gender = @gender, " +
                              "sex = @sex, " +
                              "min_age = @minAge, " +
                              "max_age = @maxAge, " +
                              "min_height = @minHeight, " +
                              "max_height = @maxHeight, " +
                              "min_weight = @minWeight, " +
                              "max_weight = @maxWeight, " +
                              "zodiac_sign = @zodiacSign, " +
                              "location = @location, " +
                              "client_id = @clientId " +
                              "WHERE requirement_id = @requirementId";
            cmd.AddParameter("gender", request.Gender)
                .AddParameter("sex", request.Sex)
                .AddParameter("minAge", request.MinAge)
                .AddParameter("maxAge", request.MaxAge)
                .AddParameter("minHeight", request.MinHeight)
                .AddParameter("maxHeight", request.MaxHeight)
                .AddParameter("minWeight", request.MinWeight)
                .AddParameter("maxWeight", request.MaxWeight)
                .AddParameter("zodiacSign", request.ZodiacSign is not null 
                    ? ZodiacSignHelper.GetUkrainianTranslation(request.ZodiacSign.Value) 
                    : null)
                .AddParameter("location", request.City)
                .AddParameter("clientId", request.ClientId)
                .AddParameter("requirementId", partnerRequirementId);

            await cmd.ExecuteNonQueryAsync();

            await transaction.CommitAsync();
            return ServiceResult<bool>.Ok(true);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return ServiceResult<bool>.BadRequest(e.Message);
        }
    }

    public async Task<ServiceResult<bool>> DeletePartnerRequirements(int id, string requestedBy)
    {
        var connection = await GetConnection(requestedBy);
        await using var transaction = await connection.BeginTransactionAsync();
        try
        {
            var cmd = transaction.CreateCommandWithAssignedTransaction();
            cmd.CommandText = "DELETE FROM partnerrequirements WHERE requirement_id = @id";
            cmd.AddParameter("id", id);

            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            if (rowsAffected == 0)
            {
                await cmd.DisposeAsync();
                await transaction.CommitAsync();
                return ServiceResult<bool>.NotFound("Вимоги до партнера", id);
            }

            await cmd.DisposeAsync();
            await transaction.CommitAsync();
            return ServiceResult<bool>.NoContent();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return ServiceResult<bool>.BadRequest(e.Message);
        }
    }

    private (string FullSqlQuery, string ConditionOnlySqlQuery) BuildSqlQueries(GetPartnersRequirementRequest request)
    {
        var selectFrom = "SELECT * FROM partnerrequirements ";
        var initialCondition = "WHERE 1=1 ";
        var idCondition = request.IdFilter.BuildConditionForInteger("requirement_id");
        var genderCondition = request.GenderFilter.BuildConditionForString("gender");
        var sexCondition = request.SexFilter.BuildConditionForString("sex");
        var minAgeCondition = request.MinAgeFilter.BuildConditionForInteger("min_age");
        var maxAgeCondition = request.MaxAgeFilter.BuildConditionForInteger("max_age");
        var minHeightCondition = request.MinHeightFilter.BuildConditionForInteger("min_height");
        var maxHeightCondition = request.MaxHeightFilter.BuildConditionForInteger("max_height");
        var minWeightCondition = request.MinWeightFilter.BuildConditionForInteger("min_weight");
        var maxWeightCondition = request.MaxWeightFilter.BuildConditionForInteger("max_weight");
        var zodiacSignCondition = request.ZodiacSignFilter.BuildConditionForString("zodiac_sign");
        var locationCondition = request.LocationFilter.BuildConditionForString("location");
        var clientIdCondition = request.ClientIdFilter.BuildConditionForInteger("client_id");
        var sortingClause = request.SortingInfo.BuildSortingString();
        var skipItems = (request.PaginationInfo.PageNumber - 1) * request.PaginationInfo.PageSize;
        var pagination = $"OFFSET {skipItems} ROWS FETCH NEXT {request.PaginationInfo.PageSize} ROWS ONLY";

        var conditionOnlySqlQuery = string.Concat(initialCondition, idCondition, genderCondition, sexCondition, 
            minAgeCondition, maxAgeCondition, minHeightCondition, maxHeightCondition, minWeightCondition, maxWeightCondition, zodiacSignCondition, locationCondition, clientIdCondition);
        return (
                string.Concat(selectFrom, conditionOnlySqlQuery, sortingClause, pagination),
                conditionOnlySqlQuery
            );
    }

    private async Task<DbConnection> GetConnection(string requestedBy)
    {
        var serviceResult = await _dbManager.GetConnection(requestedBy);
        if (!serviceResult.Success || serviceResult.ResponseData is null)
        {
            throw new InvalidOperationException(
                "Не вдалося отримати підключення до БД для даного користувача. Спробуйте увійти в аккаунт знову");
        }

        return serviceResult.ResponseData;
    }
}