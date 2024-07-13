using System.Data.Common;
using System.Globalization;
using System.Net;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.DTOs.UserManagement;
using DatingAgencyMS.Application.DTOs.UserManagement.Requests;
using DatingAgencyMS.Application.DTOs.UserManagement.Responses;
using DatingAgencyMS.Application.Shared;
using DatingAgencyMS.Domain.Models;
using DatingAgencyMS.Infrastructure.Constants;
using DatingAgencyMS.Infrastructure.Extensions;
using DatingAgencyMS.Infrastructure.Helpers;
using DatingAgencyMS.Infrastructure.SqlQueryBuilder.Common;
using DatingAgencyMS.Infrastructure.SqlQueryBuilder.Keys;

namespace DatingAgencyMS.Infrastructure.Services;

public class PostgresUserManager : IUserManager
{
    private readonly IDbManager _dbManager;

    public PostgresUserManager(IDbManager dbManager)
    {
        _dbManager = dbManager;
    }

    public async Task<ServiceResult<GetUsersResponse>> GetUsers(GetUsersRequest request)
    {
        var connection = await GetConnection(request.RequestedBy);
        await using var transaction = await connection.BeginTransactionAsync();
        List<DbUserDto> users = [];
        long? totalCount = null;
        try
        {
            var sqlQuery = BuildSqlQuery(request);
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            cmd.CommandText = sqlQuery;
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    var id = reader.GetInt32(reader.GetOrdinal("id"));
                    var login = reader.GetString(reader.GetOrdinal("login"));
                    var role = reader.GetString(reader.GetOrdinal("role"));
                    users.Add(new DbUserDto(id, login, role));
                }
            }

            cmd.CommandText = "SELECT COUNT(*) FROM keys";
            totalCount = (long?)await cmd.ExecuteScalarAsync();

            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return new ServiceResult<GetUsersResponse>(false, (int)HttpStatusCode.BadRequest, default,
                e.Message);
        }

        return new ServiceResult<GetUsersResponse>(true, (int)HttpStatusCode.OK,
            new GetUsersResponse(users.AsReadOnly(), totalCount!.Value));
    }

    private string BuildSqlQuery(GetUsersRequest request)
    {
        var builder = UserQueryBuilder.GetUserQueryBuilder();
        if (request.Id.HasValue || !string.IsNullOrEmpty(request.Login) || !string.IsNullOrEmpty(request.Role))
        {
            builder.StartConditionClause();
        }

        if (request.Id.HasValue)
        {
            builder.And();
            builder.WithIdEqual(request.Id.Value);
        }

        if (!string.IsNullOrEmpty(request.Login))
        {
            builder.And();
            builder.WithLoginLike(request.Login);
        }

        if (!string.IsNullOrEmpty(request.Role))
        {
            builder.And();
            builder.WithRoleLike(request.Role);
        }


        var pagingStage = request is { SortBy: not null, SortDirection: not null }
            ? builder.EndConditionClause().OrderBy(request.SortBy, request.SortDirection)
            : builder.EndConditionClause().OrderById();

        return request is { Page: not null, Size: not null }
            ? pagingStage.WithPaging(request.Page.Value, request.Size.Value).ToSql()
            : pagingStage.NoPaging().ToSql();
    }

    public async Task<ServiceResult<long>> CreateUser(CreateUserRequest request)
    {
        var connection = await GetConnection(request.RequestedBy);
        await using var transaction = await connection.BeginTransactionAsync();
        long? id = null;
        try
        {
            await using var cmd = connection.CreateCommand();
            cmd.Transaction = transaction;
            if (await UserWithLoginExists(request.Login, cmd))
            {
                await transaction.RollbackAsync();
                return new ServiceResult<long>(false, (int)HttpStatusCode.BadRequest, default,
                    "User with given login already exists");
            }

            var (hashedPassword, salt) = PasswordHelper.HashPasword(request.Password);
            var role = request.Role.ToString().ToUpperInvariant();
            cmd.CommandText = "INSERT INTO keys (login, password_hash, password_salt, role) VALUES " +
                              "(@login, @passwordHash, @passwordSalt, @role) RETURNING id;";
            cmd.AddParameter("login", request.Login);
            cmd.AddParameter("passwordHash", hashedPassword);
            cmd.AddParameter("passwordSalt", salt);
            cmd.AddParameter("role", role);
            id = (int?)await cmd.ExecuteScalarAsync();
            if (id is null)
            {
                await transaction.RollbackAsync();
                return new ServiceResult<long>(false, (int)HttpStatusCode.BadRequest, default,
                    "Error adding user to table");
            }

            cmd.Parameters.Clear();
            var escapedLogin = "\"" + request.Login.Replace("\"", "\"\"") + "\"";
            var escapedPassword = "'" + request.Password.Replace("'", "''") + "'";
            cmd.CommandText = $"CREATE ROLE {escapedLogin} WITH LOGIN PASSWORD {escapedPassword}";
            await cmd.ExecuteNonQueryAsync();
            await GrantAccessRights(escapedLogin, request.Role, cmd);

            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return new ServiceResult<long>(false, (int)HttpStatusCode.BadRequest, default, e.Message);
        }

        return new ServiceResult<long>(true, (int)HttpStatusCode.Created, id.Value);
    }

    private static async Task GrantAccessRights(string login, DbRoles role, DbCommand cmd)
    {
        var template = DbRolesInfo.GetGrantPrivilegesTemplateStringForRole(role);
        cmd.CommandText = string.Format(template, login);
        await cmd.ExecuteNonQueryAsync();
    }

    private static async Task<bool> UserWithLoginExists(string login, DbCommand cmd)
    {
        cmd.CommandText = "SELECT COUNT(*) FROM keys WHERE login = @login";
        cmd.AddParameter("@login", login);
        var count = (long?)await cmd.ExecuteScalarAsync();

        cmd.Parameters.Clear();
        return count == 1;
    }

    public async Task<ServiceResult<bool>> DeleteUser(DeleteUserRequest request)
    {
        var connection = await GetConnection(request.RequestedBy);
        var success = false;
        await using var transaction = await connection.BeginTransactionAsync();
        try
        {
            await using var cmd = connection.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandText = "DELETE FROM keys WHERE login = @login";
            cmd.AddParameter("login", request.Login);
            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            success = rowsAffected == 1;
            await transaction.CommitAsync();
        }
        catch (DbException e)
        {
            await transaction.RollbackAsync();
            return new ServiceResult<bool>(false, (int)HttpStatusCode.BadRequest, default, e.Message);
        }

        return new ServiceResult<bool>(success, (int)HttpStatusCode.OK, success,
            !success ? "User with given id was not found" : "");
    }

    public async Task<ServiceResult<bool>> AssignNewRole(AssignNewRoleRequest request)
    {
        var connection = await GetConnection(request.RequestedBy);
        var success = false;
        await using var transaction = await connection.BeginTransactionAsync();
        try
        {
            await using var cmd = connection.CreateCommand();
            cmd.Transaction = transaction;
            cmd.CommandText = "UPDATE keys SET role = @role WHERE login = @login;";
            cmd.AddParameter("role", request.NewRole.ToString().ToUpperInvariant());
            cmd.AddParameter("login", request.Login);
            success = await cmd.ExecuteNonQueryAsync() == 1;
            await transaction.CommitAsync();
        }
        catch (DbException e)
        {
            await transaction.RollbackAsync();
            return new ServiceResult<bool>(false, (int)HttpStatusCode.BadRequest, default, e.Message);
        }

        return new ServiceResult<bool>(success, (int)HttpStatusCode.OK, success);
    }

    public async Task<ServiceResult<DbRoles>> GetUserRole(string login)
    {
        var connection = await GetConnection(login);
        await using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT role FROM keys WHERE login=@login";
        cmd.AddParameter("login", login);
        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            var role = reader.GetString(reader.GetOrdinal("role"));
            return new ServiceResult<DbRoles>(true, (int)HttpStatusCode.OK, Enum.Parse<DbRoles>(role, true));
        }

        throw new ArgumentException("User with given login was not found", login);
    }

    private async Task<DbConnection> GetConnection(string requestedBy)
    {
        var serviceResult = await _dbManager.GetConnection(requestedBy);
        if (!serviceResult.Success || serviceResult.ResponseData is null)
        {
            throw new InvalidOperationException(
                "Could not get access to database connection for specified user. Try to log in again.");
        }

        return serviceResult.ResponseData;
    }
}