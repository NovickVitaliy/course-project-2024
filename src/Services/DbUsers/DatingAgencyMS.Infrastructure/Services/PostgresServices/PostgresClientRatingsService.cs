using System.Data;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.DTOs.ClientRatings;
using DatingAgencyMS.Application.DTOs.ClientRatings.Requests;
using DatingAgencyMS.Application.DTOs.ClientRatings.Responses;
using DatingAgencyMS.Application.Shared;
using DatingAgencyMS.Infrastructure.Extensions;
using DatingAgencyMS.Infrastructure.Helpers;

namespace DatingAgencyMS.Infrastructure.Services.PostgresServices;

public class PostgresClientRatingsService : IClientRatingsService
{
    private readonly IDbManager _dbManager;

    public PostgresClientRatingsService(IDbManager dbManager)
    {
        _dbManager = dbManager;
    }

    public async Task<ServiceResult<int>> CreateClientRatingAsync(CreateClientRatingRequest request)
    {
        var connection = await _dbManager.GetConnectionOrThrow();
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            cmd.CommandText = "INSERT INTO clientratings (client_id, rating, comment, rating_date) " +
                              "VALUES (@clientId, @rating, @comment, @ratingDate) RETURNING rating_id";
            cmd.AddParameter("clientId", request.ClientId)
                .AddParameter("rating", request.Rating)
                .AddParameter("comment", request.Comment)
                .AddParameter("ratingData", DateTime.UtcNow);

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

    public async Task<ServiceResult<GetClientRatingsResponse>> GetClientRatingsAsync(GetClientRatingsRequest request)
    {
        List<ClientRatingDto> dtos = [];
        var connection = await _dbManager.GetConnectionOrThrow();
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {
            var sql = BuildSqlQuery(request);
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            cmd.CommandText = sql.FullSql;
            var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var id = reader.GetInt32("rating_id");
                var clientId = reader.GetInt32("client_id");
                var rating = reader.GetInt32("rating");
                var comment = reader.GetString("comment");
                var ratingDate = reader.GetDateTime("rating_date");

                dtos.Add(new ClientRatingDto(id, clientId, rating, comment, ratingDate));
            }

            await reader.CloseAsync();

            cmd.Parameters.Clear();

            cmd.CommandText = $"SELECT COUNT(*) FROM clientratings {sql.ConditionSql}";
            var count = (long?)await cmd.ExecuteScalarAsync();

            await transaction.CommitAsync();
            return ServiceResult<GetClientRatingsResponse>.Ok(new GetClientRatingsResponse(dtos.ToArray(), count!.Value));
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return ServiceResult<GetClientRatingsResponse>.ServerError(e.Message);
        }
    }
    private (string FullSql, string ConditionSql) BuildSqlQuery(GetClientRatingsRequest request)
    {
        const string selectFrom = "SELECT * FROM clientratings ";
        const string initialCondition = "WHERE 1=1 ";
        var idCondition = request.IdFilter.BuildConditionForInteger("rating_id");
        var clientIdCondition = request.ClientIdFilter.BuildConditionForInteger("client_id");
        var ratingCondition = request.RatingFilter.BuildConditionForInteger("rating");
        var commentCondition = request.CommentFilter.BuildConditionForString("comment");
        var ratingDateCondition = request.RatingDateFilter.BuildConditionForDateTime("rating_date");
        var sortingString = request.SortingInfo.BuildSortingString();
        var skipItems = ((request.PaginationInfo.PageNumber - 1) * request.PaginationInfo.PageSize);
        var pagination = $"OFFSET {skipItems} ROWS FETCH NEXT {request.PaginationInfo.PageSize} ROWS ONLY";

        var conditionSql = string.Concat(initialCondition, idCondition, clientIdCondition,
            ratingCondition, commentCondition, ratingDateCondition);

        return (string.Concat(selectFrom, conditionSql, sortingString, pagination), conditionSql);
    }

    public async Task<ServiceResult<ClientRatingDto>> GetClientRatingByIdAsync(int id)
    {
        var connection = await _dbManager.GetConnectionOrThrow();
        await using var transaction = await connection.BeginTransactionAsync();
        try
        {
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            cmd.CommandText = "SELECT * FROM clientratings WHERE rating_id = @ratingId";
            cmd.AddParameter("ratingId", id);
            var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync(default))
            {
                await reader.CloseAsync();
                await transaction.RollbackAsync();
                return ServiceResult<ClientRatingDto>.NotFound("Відгук клієнта", id);
            }

            var clientId = reader.GetInt32("client_id");
            var rating = reader.GetInt32("rating");
            var comment = reader.GetString("comment");
            var ratingDate = reader.GetDateTime("rating_date");

            await reader.CloseAsync();
            await transaction.CommitAsync();

            return ServiceResult<ClientRatingDto>.Ok(new ClientRatingDto(id, clientId, rating, comment, ratingDate));
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return ServiceResult<ClientRatingDto>.ServerError(e.Message);
        }
    }

    public async Task<ServiceResult<bool>> UpdateClientRatingAsync(UpdateClientRatingRequest request)
    {
        var connection = await _dbManager.GetConnectionOrThrow();
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            cmd.CommandText = "SELECT COUNT(*) FROM clientratings WHERE rating_id = @ratingId";
            cmd.AddParameter("ratingId", request.Id);

            var rowsFound = (long?)await cmd.ExecuteScalarAsync();
            if (rowsFound!.Value != 1)
            {
                await transaction.RollbackAsync();
                return ServiceResult<bool>.NotFound("Відгук клієнта", request.Id);
            }

            cmd.Parameters.Clear();

            cmd.CommandText = "UPDATE clientratings " +
                              "SET rating = @rating, " +
                              "comment = @comment " +
                              "WHERE rating_id = @ratingId";
            cmd.AddParameter("rating", request.Rating)
                .AddParameter("comment", request.Comment)
                .AddParameter("ratingId", request.Id);

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

    public async Task<ServiceResult<bool>> DeleteClientRatingAsync(int id)
    {
        var connection = await _dbManager.GetConnectionOrThrow();
        await using var transaction = await connection.BeginTransactionAsync(IsolationLevel.Serializable);
        try
        {
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            cmd.CommandText = "DELETE FROM clientratings WHERE rating_id = @ratingId";
            cmd.AddParameter("ratingId", id);

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