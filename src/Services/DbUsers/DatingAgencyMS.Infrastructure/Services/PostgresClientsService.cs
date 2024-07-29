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
                var sex = reader.GetString("sex");
                var sexualOrientation = reader.GetString("sexual_orientation");
                var registrationNumber = reader.GetString("registration_number");
                var registeredOn = DateOnly.FromDateTime(reader.GetDateTime("registered_on"));
                var age = reader.GetInt32("age");
                var height = reader.GetInt32("height");
                var weight = reader.GetInt32("weight");
                var zodiacSign = ZodiacSignHelper.FromUkrainianToZodiacSign(reader.GetString("zodiac_sign"));
                var description = reader.GetString("description");
                var hasDeclinedService = reader.GetBoolean("has_declined_service");

                clientDtos.Add(new ClientDto(clientId, firstName, lastName, gender, sex, sexualOrientation,
                    registrationNumber, registeredOn,
                    age, height, weight, zodiacSign, description, hasDeclinedService));
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

    private static (string FullQuery, string WhereQuery) BuildClientsSqlQuery(GetClientsRequest request)
    {
        var selectFrom = "SELECT * FROM Clients ";
        var initialCondition = "WHERE 1=1 ";
        var idCondition = request.IdFilter.BuildConditionForInteger("id");
        var firstNameCondition = request.FirstNameFilter.BuildConditionForString("first_name");
        var lastNameCondition = request.LastNameFilter.BuildConditionForString("last_name");
        var genderCondition = request.GenderFilter.BuildConditionForString("gender");
        var sexCondition = request.SexFilter.BuildConditionForString("sex");
        var sexualOrientationCondition = request.SexualOrientationFilter.BuildConditionForString("sexual_orientation");
        var registrationNumberCondition =
            request.RegistrationNumberFilter.BuildConditionForString("registration_number");
        var registeredOnCondition = request.RegisteredOnFilter.BuildConditionForDateOnly("registered_on");
        var ageCondition = request.AgeFilter.BuildConditionForInteger("age");
        var heightCondition = request.HeightFilter.BuildConditionForInteger("height");
        var weightCondition = request.WeightFilter.BuildConditionForInteger("weight");
        var zodiacSignCondition = request.ZodiacSignFilter.BuildConditionForString("zodiac_sign");
        var descriptionFilter = request.DescriptionFilter.BuildConditionForString("description");
        var hasDeclinedServiceFilter =
            request.HasDeclinedServiceFilter.BuildConditionForBoolean("has_declined_service");
        var sortingString = request.SortingInfo.BuildSortingString();

        var skipItems = ((request.PaginationInfo.PageNumber - 1) * request.PaginationInfo.PageSize);
        var pagination = $"OFFSET {skipItems} ROWS FETCH NEXT {request.PaginationInfo.PageSize} ROWS ONLY";

        return (string.Concat(selectFrom, initialCondition, idCondition, firstNameCondition, lastNameCondition,
                genderCondition, sexCondition, sexualOrientationCondition, registrationNumberCondition, registeredOnCondition,
                ageCondition, heightCondition,
                weightCondition, zodiacSignCondition, descriptionFilter, hasDeclinedServiceFilter, sortingString, pagination),
            string.Concat(initialCondition, idCondition, firstNameCondition, lastNameCondition, genderCondition, sexCondition,
                sexualOrientationCondition, registrationNumberCondition, registeredOnCondition, ageCondition,
                heightCondition,
                weightCondition, zodiacSignCondition, descriptionFilter, hasDeclinedServiceFilter));
    }

    public async Task<ServiceResult<CreateClientResponse>> CreateClient(CreateClientRequest request)
    {
        var connection = await GetConnection(request.RequestedBy);
        await using var transaction = await connection.BeginTransactionAsync();
        try
        {
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            cmd.CommandText =
                "INSERT INTO clients (first_name, last_name, gender, sex, sexual_orientation, registration_number, registered_on, age, height, weight, zodiac_sign, description) " +
                "VALUES (@firstName, @lastName, @gender, @sex, @sexualOrientation, @registrationNumber, @registeredOn, @age, @height, @weight, @zodiacSign, @description)";
            cmd.AddParameter("firstName", request.FirstName);
            cmd.AddParameter("lastName", request.LastName);
            cmd.AddParameter("gender", request.Gender);
            cmd.AddParameter("sex", request.Sex);
            cmd.AddParameter("sexualOrientation", request.SexualOrientation);
            cmd.AddParameter("registrationNumber", request.RegistrationNumber);
            cmd.AddParameter("registeredOn", DateOnly.FromDateTime(DateTime.Today));
            cmd.AddParameter("age", request.Age);
            cmd.AddParameter("height", request.Height);
            cmd.AddParameter("weight", request.Weight);
            cmd.AddParameter("zodiacSign", request.ZodiacSign.ToString());
            cmd.AddParameter("description", request.Description);

            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            await transaction.CommitAsync();

            if (rowsAffected != 1)
            {
                return ServiceResult<CreateClientResponse>.BadRequest("Невдала спроба створення клієнта.");
            }

            return ServiceResult<CreateClientResponse>.Created(new CreateClientResponse(true));
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return ServiceResult<CreateClientResponse>.BadRequest(e.Message);
        }
    }

    public async Task<ServiceResult<bool>> DeleteClient(int clientId, string requestedBy)
    {
        var connection = await GetConnection(requestedBy);
        await using var transaction = await connection.BeginTransactionAsync();
        try
        {
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            cmd.CommandText = "DELETE FROM clients WHERE id = @clientId";
            cmd.AddParameter("clientId", clientId);

            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            await transaction.CommitAsync();

            if (rowsAffected == 0)
            {
                return ServiceResult<bool>.NotFound("Клієнт", clientId);
            }

            return ServiceResult<bool>.NoContent();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return ServiceResult<bool>.BadRequest(e.Message);
        }
    }

    public async Task<ServiceResult<bool>> UpdateClient(int clientId, UpdateClientRequest request)
    {
        var connection = await GetConnection(request.RequestedBy);
        await using var transaction = await connection.BeginTransactionAsync();
        try
        {
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            cmd.CommandText = "UPDATE clients SET " +
                              "first_name = @firstName, " +
                              "last_name = @lastName, " +
                              "gender = @gender, " +
                              "sex = @sex," +
                              "sexual_orientation = @sexualOrientation, " +
                              "registration_number = @registrationNumber, " +
                              "age = @age, " +
                              "height = @height, " +
                              "weight = @weight, " +
                              "zodiac_sign = @zodiacSign," +
                              "description = @description," +
                              "has_declined_service = @hasDeclinedService " +
                              "WHERE id = @clientId";
            cmd.AddParameter("firstName", request.FirstName);
            cmd.AddParameter("lastName", request.LastName);
            cmd.AddParameter("gender", request.Gender);
            cmd.AddParameter("sex", request.Sex);
            cmd.AddParameter("sexualOrientation", request.SexualOrientation);
            cmd.AddParameter("registrationNumber", request.RegistrationNumber);
            cmd.AddParameter("age", request.Age);
            cmd.AddParameter("height", request.Height);
            cmd.AddParameter("weight", request.Weight);
            cmd.AddParameter("zodiacSign", ZodiacSignHelper.GetUkrainianTranslation(request.ZodiacSign));
            cmd.AddParameter("description", request.Description);
            cmd.AddParameter("hasDeclinedService", request.HasDeclinedService);
            cmd.AddParameter("clientId", clientId);

            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            await transaction.CommitAsync();

            if (rowsAffected == 0)
            {
                return ServiceResult<bool>.BadRequest("Не вдалося оновити запис клієнта");
            }

            return ServiceResult<bool>.NoContent();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return ServiceResult<bool>.BadRequest(e.Message);
        }
    }

    public async Task<ServiceResult<GetClientResponse>> GetClientById(GetClientRequest getClientRequest)
    {
        var connection = await GetConnection(getClientRequest.RequestedBy);
        await using var transaction = await connection.BeginTransactionAsync();
        try
        {
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            cmd.CommandText = "SELECT * FROM clients WHERE id = @clientId";
            cmd.AddParameter("clientId", getClientRequest.ClientId);
            var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                await reader.CloseAsync();
                return ServiceResult<GetClientResponse>.NotFound("Клієнт", getClientRequest.ClientId);
            }
            
            var clientId = getClientRequest.ClientId;
            var firstName = reader.GetString("first_name");
            var lastName = reader.GetString("last_name");
            var gender = reader.GetString("gender");
            var sex = reader.GetString("sex");
            var sexualOrientation = reader.GetString("sexual_orientation");
            var registrationNumber = reader.GetString("registration_number");
            var registeredOn = DateOnly.FromDateTime(reader.GetDateTime("registered_on"));
            var age = reader.GetInt32("age");
            var height = reader.GetInt32("height");
            var weight = reader.GetInt32("weight");
            var zodiacSign = ZodiacSignHelper.FromUkrainianToZodiacSign(reader.GetString("zodiac_sign"));
            var description = reader.GetString("description");
            var hasDeclinedService = reader.GetBoolean("has_declined_service");
            await reader.CloseAsync();
            await transaction.CommitAsync();

            var clientDto = new ClientDto(clientId, firstName, lastName, gender, sex, sexualOrientation, registrationNumber,
                registeredOn, age, height, weight, zodiacSign, description, hasDeclinedService);
            return ServiceResult<GetClientResponse>.Ok(new GetClientResponse(clientDto));
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return ServiceResult<GetClientResponse>.BadRequest(e.Message);
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