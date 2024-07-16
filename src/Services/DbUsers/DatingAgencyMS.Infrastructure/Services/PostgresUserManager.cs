using System.Data.Common;
using System.Net;
using System.Text;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.DTOs;
using DatingAgencyMS.Application.DTOs.UserManagement;
using DatingAgencyMS.Application.DTOs.UserManagement.Requests;
using DatingAgencyMS.Application.DTOs.UserManagement.Responses;
using DatingAgencyMS.Application.Shared;
using DatingAgencyMS.Domain.Models;
using DatingAgencyMS.Infrastructure.Constants;
using DatingAgencyMS.Infrastructure.Extensions;
using DatingAgencyMS.Infrastructure.Helpers;

namespace DatingAgencyMS.Infrastructure.Services;

public class PostgresUserManager : IUserManager
{
    private readonly IDbManager _dbManager;

    public PostgresUserManager(IDbManager dbManager)
    {
        _dbManager = dbManager;
    }

    public async Task<ServiceResult<bool>> CheckUserCredentials(LoginDbRequest loginDbRequest)
    {
        var connection = await _dbManager.GetRootConnection();
        await using var transaction = await connection.BeginTransactionAsync();
        try
        {
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            cmd.CommandText = "SELECT * FROM keys WHERE login = @login";
            cmd.AddParameter("login", loginDbRequest.Login);
            
            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var passwordHash = reader.GetString(reader.GetOrdinal("password_hash"));
                var passwordSalt = reader.GetString(reader.GetOrdinal("password_salt"));
                var equals = PasswordHelper.VerifyPassword(loginDbRequest.Password, passwordHash, passwordSalt);
                if (equals)
                {
                    return new ServiceResult<bool>(true, (int)HttpStatusCode.OK, true);
                }

                return new ServiceResult<bool>(false, (int)HttpStatusCode.BadRequest, true, "Неправильний пароль");
            }

            return new ServiceResult<bool>(false, (int)HttpStatusCode.NotFound, false,
                "Користувач з даним логіном не був знайдений");
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return new ServiceResult<bool>(false, (int)HttpStatusCode.BadRequest, false, e.Message);
        }
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
        var builder = new StringBuilder("SELECT * FROM keys WHERE 1=1 ");
        
        if (request.Id.HasValue)
        {
            builder.Append($"AND id={request.Id} ");
        }

        if (!string.IsNullOrEmpty(request.Login))
        {
            builder.Append($"AND LOWER(login) LIKE LOWER('%{request.Login}%') ");
        }

        if (!string.IsNullOrEmpty(request.Role))
        {
            builder.Append($"AND LOWER(role) LIKE LOWER('%{request.Role}%') ");
        }


        builder = request is { SortBy: not null, SortDirection: not null }
            ? builder.Append($"ORDER BY {request.SortBy} {request.SortDirection} ")
            : builder.Append(' ');

        var offset = (request.Page - 1) * request.Size;

        return request is { Page: not null, Size: not null }
            ? builder.Append($"OFFSET {offset} ROWS FETCH NEXT {request.Size} ROWS ONLY ").ToString()
            : builder.Append(' ').ToString();
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
                    "Користувач з даним логіном вже існує");
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
                    "Помилка при створенні користувача");
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
            !success ? "Користувач з таким Id не був знайдений" : "");
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

        throw new ArgumentException("Користувач з таким логіном не був знайдений", login);
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