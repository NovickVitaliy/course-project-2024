using System.Data.Common;
using System.Data;
using System.Text;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.DTOs;
using DatingAgencyMS.Application.DTOs.UserManagement;
using DatingAgencyMS.Application.DTOs.UserManagement.Requests;
using DatingAgencyMS.Application.DTOs.UserManagement.Responses;
using DatingAgencyMS.Application.Shared;
using DatingAgencyMS.Domain.Models;
using DatingAgencyMS.Domain.Models.DbManagement;
using DatingAgencyMS.Infrastructure.Constants;
using DatingAgencyMS.Infrastructure.Extensions;
using DatingAgencyMS.Infrastructure.Helpers;
using Npgsql;

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

            var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                await reader.DisposeAsync();
                await transaction.RollbackAsync();
                return ServiceResult<bool>.NotFound("Користувач БД", loginDbRequest.Login);
            }
            
            var passwordHash = reader.GetString(reader.GetOrdinal("password_hash"));
            var passwordSalt = reader.GetString(reader.GetOrdinal("password_salt"));
            
            await reader.DisposeAsync();
            
            var equals = PasswordHelper.VerifyPassword(loginDbRequest.Password, passwordHash, passwordSalt);
            if (equals)
            {
                await transaction.CommitAsync();
                return ServiceResult<bool>.Ok(true);
            }

            await transaction.RollbackAsync();
            return ServiceResult<bool>.BadRequest("Неправильний пароль");
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return ServiceResult<bool>.BadRequest(e.Message);
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
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            var sqlQuery = BuildSqlQuery(request);
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
            return ServiceResult<GetUsersResponse>.BadRequest(e.Message);
        }

        return ServiceResult<GetUsersResponse>.Ok(new GetUsersResponse(users.AsReadOnly(), totalCount!.Value));
    }

    public async Task<ServiceResult<GetUserResponse>> GetUser(GetUserRequest request)
    {
        var connection = await GetConnection(request.RequestedBy);
        await using var transaction = await connection.BeginTransactionAsync();
        try
        {
            var cmd = transaction.CreateCommandWithAssignedTransaction();
            cmd.CommandText = "SELECT * FROM keys WHERE login = @login";
            cmd.AddParameter("login", request.Login);
            var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                await reader.DisposeAsync();
                await transaction.RollbackAsync();
                return ServiceResult<GetUserResponse>.NotFound("Користувач БД", request.Login);
            }
            var id = reader.GetInt32("id");
            var login = reader.GetString("login");
            var role = reader.GetString("role");
            
            await reader.DisposeAsync();
            await transaction.CommitAsync();
            
            var response = new GetUserResponse(new DbUserDto(id, login, role));
            return ServiceResult<GetUserResponse>.Ok(response);
        }
        catch (DbException e)
        {
            await transaction.RollbackAsync();
            return ServiceResult<GetUserResponse>.BadRequest(e.Message);
        }
    }

    private static string BuildSqlQuery(GetUsersRequest request)
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
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            
            if (await UserWithLoginExists(request.Login, cmd))
            {
                await transaction.RollbackAsync();
                return ServiceResult<long>.BadRequest("Користувач з даним логіном вже існує");
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
                return ServiceResult<long>.BadRequest("Помилка при створенні користувача");
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
            return ServiceResult<long>.BadRequest(e.Message);
        }

        return ServiceResult<long>.Created(id.Value);
    }

    private static async Task GrantAccessRights(string login, DbRoles role, DbCommand cmd)
    {
        var cmdText = DbRolesInfo.GetGrantRoleForUserString(role, login);
        cmd.CommandText = cmdText;
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
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            var (allowed, serviceResult) = await IsAllowedToDeleteUser(cmd, request.Login, request.RequestedBy);
            if (!allowed)
            {
                await transaction.RollbackAsync();
                return serviceResult 
                       ?? ServiceResult<bool>.Forbidden("Ви не можете видаляти користувача який має роль таку ж як і ви або вище");
            }
            
            cmd.CommandText = "DELETE FROM keys WHERE login = @login";
            cmd.AddParameter("login", request.Login);
            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            
            cmd.Parameters.Clear();
            cmd.CommandText = $"DROP ROLE {request.Login}";
            await cmd.ExecuteNonQueryAsync();
            
            success = rowsAffected == 1;
            await transaction.CommitAsync();
        }
        catch (DbException e)
        {
            await transaction.RollbackAsync();
            return ServiceResult<bool>.BadRequest(e.Message);
        }

        return success
            ? ServiceResult<bool>.Ok(success)
            : ServiceResult<bool>.NotFound("Користувач БД", request.Login);
    }

    private static async Task<(bool Allowed, ServiceResult<bool>? ServiceResult)> IsAllowedToDeleteUser(DbCommand cmd, string login, string requestedBy)
    {
        var (userToDeleteRole, serviceResult) = await ReadUserRole(cmd, login);
        if (serviceResult is not null)
        {
            return (false, serviceResult);
        }

        (var requestedByRole, serviceResult) = await ReadUserRole(cmd, requestedBy);
        if (serviceResult is not null)
        {
            return (false, serviceResult);
        }

        //comparing like this because of the way the enum was created
        return (requestedByRole < userToDeleteRole, null);
    }

    private static async Task<(DbRoles? Role, ServiceResult<bool>? serviceResult)> ReadUserRole(DbCommand cmd, string login)
    {
        cmd.CommandText = "SELECT role FROM keys WHERE login = @login";
        cmd.AddParameter("login", login);

        await using var reader = await cmd.ExecuteReaderAsync();
        cmd.Parameters.Clear();
        if (await reader.ReadAsync())
        {
            try
            {
                var role = Enum.Parse<DbRoles>(reader.GetString(reader.GetOrdinal("role")), true);
                return (role, null);
            }
            catch (Exception e)
            {
                //TODO: log somewhere sometime
                return (null, ServiceResult<bool>.ServerError(e.Message));
            }
        }
        
        return (null, ServiceResult<bool>.NotFound("Користувач БД", login));
    }

    public async Task<ServiceResult<bool>> AssignNewRole(AssignNewRoleRequest request)
    {
        var connection = await GetConnection(request.RequestedBy);
        var success = false;
        await using var transaction = await connection.BeginTransactionAsync();
        try
        {
            await using var cmd = transaction.CreateCommandWithAssignedTransaction();
            
            cmd.CommandText = "UPDATE keys SET role = @role WHERE login = @login;";
            cmd.AddParameter("role", request.NewRole.ToString().ToUpperInvariant());
            cmd.AddParameter("login", request.Login);
            
            success = await cmd.ExecuteNonQueryAsync() == 1;
            if (!success)
            {
                await transaction.RollbackAsync();
                return ServiceResult<bool>.BadRequest("Не вдалося поміняти роль для користувача");
            }
            
            cmd.Parameters.Clear();
            
            cmd.CommandText = DbRolesInfo.GetRevokeRoleFromUserString(request.OldRole, request.Login);
            await cmd.ExecuteNonQueryAsync();
            
            cmd.CommandText = DbRolesInfo.GetGrantRoleForUserString(request.NewRole, request.Login);
            await cmd.ExecuteNonQueryAsync();
            
            await transaction.CommitAsync();
        }
        catch (DbException e)
        {
            await transaction.RollbackAsync();
            return ServiceResult<bool>.BadRequest(e.Message);
        }

        return ServiceResult<bool>.Ok(success);
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
            return ServiceResult<DbRoles>.Ok(Enum.Parse<DbRoles>(role, true));
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