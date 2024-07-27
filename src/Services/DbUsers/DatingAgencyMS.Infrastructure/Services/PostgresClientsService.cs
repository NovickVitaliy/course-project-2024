using System.Data;
using System.Data.Common;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.DTOs.Clients;
using DatingAgencyMS.Application.DTOs.Clients.Requests;
using DatingAgencyMS.Application.DTOs.Clients.Responses;
using DatingAgencyMS.Application.Shared;
using DatingAgencyMS.Domain.Models.Business;
using DatingAgencyMS.Infrastructure.Extensions;
using DatingAgencyMS.Infrastructure.Helpers;

namespace DatingAgencyMS.Infrastructure.Services;

public class PostgresClientsService : IClientsService
{
    private readonly IDbManager _dbManager;

    public PostgresClientsService(IDbManager dbManager)
    {
        _dbManager = dbManager;
    }

    public async Task<ServiceResult<GetClientsResponse>> GetClients(GetClientsRequest request)
    {
        var connection = await GetConnection(request.RequestedBy);
        await using var transaction = await connection.BeginTransactionAsync();
        try
        {
            var sql = BuildClientsSqlQuery(request);
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            cmd.CommandText = sql.FullQuery;

            List<ClientDto> clientDtos = [];
            var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var clientId = reader.GetInt32("id");
                var firstName = reader.GetString("first_name");
                var lastName = reader.GetString("last_name");
                var gender = reader.GetString("gender");
                var sexualOrientation = reader.GetString("sexual_orientation");
                var registrationNumber = reader.GetString("registration_number");
                var registeredOn = DateOnly.FromDateTime(reader.GetDateTime("registered_on"));
                var age = reader.GetInt32("age");
                var height = reader.GetInt32("height");
                var weight = reader.GetInt32("weight");
                var zodiacSign = Enum.Parse<ZodiacSign>(reader.GetString("zodiac_sign"), true);
                var description = reader.GetString("description");

                clientDtos.Add(new ClientDto(clientId, firstName, lastName, gender, sexualOrientation, registrationNumber, registeredOn,
                    age, height, weight, zodiacSign, description));
            }

            await reader.CloseAsync();

            cmd.CommandText = string.Concat("SELECT COUNT(*) FROM Clients ", sql.WhereQuery);
            var totalCount = (long)await cmd.ExecuteScalarAsync()!;
            
            await transaction.CommitAsync();
            return ServiceResult<GetClientsResponse>.Ok(new GetClientsResponse(clientDtos, totalCount));
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return ServiceResult<GetClientsResponse>.BadRequest(e.Message);
        }
    }

    private (string FullQuery, string WhereQuery) BuildClientsSqlQuery(GetClientsRequest request)
    {
        var selectFrom = "SELECT * FROM Clients ";
        var initialCondition = "WHERE 1=1 ";
        var idCondition = request.IdFilter.BuildConditionForInteger("id");
        var firstNameCondition = request.FirstNameFilter.BuildConditionForString("first_name");
        var lastNameCondition = request.LastNameFilter.BuildConditionForString("last_name");
        var genderCondition = request.GenderFilter.BuildConditionForString("gender");
        var sexualOrientationCondition = request.SexualOrientationFilter.BuildConditionForString("sexual_orientation");
        var registrationNumberCondition = request.RegistrationNumberFilter.BuildConditionForString("registration_number");
        var registeredOnCondition = request.RegisteredOnFilter.BuildConditionForDateOnly("registered_on");
        var ageCondition = request.AgeFilter.BuildConditionForInteger("age");
        var heightCondition = request.HeightFilter.BuildConditionForInteger("height");
        var weightCondition = request.WeightFilter.BuildConditionForInteger("weight");
        var zodiacSignCondition = request.ZodiacSignFilter.BuildConditionForString("zodiac_sign");
        var descriptionFilter = request.DescriptionFilter.BuildConditionForString("description");
        var sortingString = request.SortingInfo.BuildSortingString();
        
        var skipItems = ((request.PaginationInfo.PageNumber - 1) * request.PaginationInfo.PageSize);
        var pagination = $"OFFSET {skipItems} ROWS FETCH NEXT {request.PaginationInfo.PageSize} ROWS ONLY";

        return (string.Concat(selectFrom, initialCondition, idCondition, firstNameCondition, lastNameCondition, genderCondition, sexualOrientationCondition, registrationNumberCondition, registeredOnCondition, ageCondition, heightCondition,
            weightCondition, zodiacSignCondition, descriptionFilter, sortingString, pagination),
            string.Concat(initialCondition, idCondition, firstNameCondition, lastNameCondition, genderCondition, sexualOrientationCondition, registrationNumberCondition, registeredOnCondition, ageCondition, heightCondition,
                weightCondition, zodiacSignCondition, descriptionFilter));
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