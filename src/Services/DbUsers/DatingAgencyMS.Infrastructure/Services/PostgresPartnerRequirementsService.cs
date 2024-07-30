using System.Data.Common;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.DTOs.PartnerRequirements.Requests;
using DatingAgencyMS.Application.Shared;
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
            cmd.AddParameter("zodiacSign", request.ZodiacSign is null ? null : request.ZodiacSign.ToString());
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