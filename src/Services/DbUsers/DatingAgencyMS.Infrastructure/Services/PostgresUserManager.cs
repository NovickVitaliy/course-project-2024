using System.Data.Common;
using System.Net;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.DTOs.UserManagement;
using DatingAgencyMS.Application.Shared;
using DatingAgencyMS.Infrastructure.Helpers;

namespace DatingAgencyMS.Infrastructure.Services;

public class PostgresUserManager : IUserManager
{
    private readonly IDbManager _dbManager;
    private readonly IRoleManager _roleManager;

    public PostgresUserManager(IDbManager dbManager, IRoleManager roleManager)
    {
        _dbManager = dbManager;
        _roleManager = roleManager;
    }

    public async Task<ServiceResult<long>> CreateUser(CreateUserRequest request)
    {
        var serviceResult = await _dbManager.GetConnection(request.RequestedBy);
        if (!serviceResult.Success)
            return new ServiceResult<long>(false, (int)HttpStatusCode.BadRequest, default, serviceResult.Description);

        var connection = serviceResult.ResponseData!;
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
            var roleId = await GetRoleId(request.Role, cmd);
            cmd.CommandText = "INSERT INTO keys (login, password_hash, password_salt, role_id) VALUES " +
                              "(@login, @passwordHash, @passwordSalt, @roleId) RETURNING id;";
            cmd.AddParameter("login", request.Login);
            cmd.AddParameter("passwordHash", hashedPassword);
            cmd.AddParameter("passwordSalt", salt);
            cmd.AddParameter("roleId", roleId);
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
            await GrantAccessRights(escapedLogin, request.Role, cmd);
            await cmd.ExecuteNonQueryAsync();
            
            await transaction.CommitAsync();
        }
        catch(Exception e)
        {
            await transaction.RollbackAsync();
            return new ServiceResult<long>(false, (int)HttpStatusCode.BadRequest, default, e.Message);
        }
        return new ServiceResult<long>(true, (int)HttpStatusCode.OK, id.Value);
    }

    private async Task GrantAccessRights(string login, string role, DbCommand cmd)
    {
        //TODO: grant access rights
    }

    private static async Task<int> GetRoleId(string role, DbCommand cmd)
    {
        //TODO: change to use service
        cmd.CommandText = "SELECT id FROM roles WHERE name = @role";
        cmd.AddParameter("role", role);
        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            cmd.Parameters.Clear();
            return reader.GetInt32(reader.GetOrdinal("id"));
        }
        //TODO: throw exception that role does exist
        return 0;
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
        var serviceResult = await _dbManager.GetConnection(request.RequestedBy);
        if (!serviceResult.Success)
            return new ServiceResult<bool>(false, (int)HttpStatusCode.BadRequest, default, serviceResult.Description);

        var connection = serviceResult.ResponseData!;
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
        
        return new ServiceResult<bool>(success, (int)HttpStatusCode.OK, success, !success ? "User with given id was not found" : "");
    }

    public async Task<ServiceResult<bool>> AssignNewRole(AssignNewRoleRequest request)
    {
        var serviceResult = await _dbManager.GetConnection(request.RequestedBy);
        if (!serviceResult.Success)
            return new ServiceResult<bool>(false, (int)HttpStatusCode.BadRequest, default, serviceResult.Description);

        var connection = serviceResult.ResponseData!;
        var success = false;
        await using var transaction = await connection.BeginTransactionAsync();
        try
        {
            await using var cmd = connection.CreateCommand();
            cmd.Transaction = transaction;
            var roleId = await GetRoleId(request.NewRole, cmd);
            cmd.CommandText = "UPDATE keys SET role_id = @roleId WHERE login = @login;";
            cmd.AddParameter("roleId", roleId);
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
}