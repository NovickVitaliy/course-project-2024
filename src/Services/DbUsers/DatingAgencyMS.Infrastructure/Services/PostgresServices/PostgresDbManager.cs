using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.Shared;
using DatingAgencyMS.Infrastructure.Extensions;
using DatingAgencyMS.Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Npgsql;

namespace DatingAgencyMS.Infrastructure.Services.PostgresServices;

public class PostgresDbManager : IDbManager, IAsyncDisposable
{
    private bool _disposed = false;
    private readonly string _pgConnTemplate;
    private readonly ConcurrentDictionary<string, DbConnectionInfo> _connections;
    private readonly DbConnection _rootConnection;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public PostgresDbManager(string pgConnTemplate, string pgRootConn, IHttpContextAccessor httpContextAccessor)
    {
        _pgConnTemplate = pgConnTemplate;
        _httpContextAccessor = httpContextAccessor;
        _connections = new ConcurrentDictionary<string, DbConnectionInfo>();
        _rootConnection = new NpgsqlConnection(pgRootConn);
    }


    public async Task<DbConnection> GetRootConnection()
    {
        if (_rootConnection.State == ConnectionState.Closed)
        {
            await _rootConnection.OpenAsync();
        }

        return _rootConnection;
    }

    public async Task<ServiceResult<bool>> TryAccessDb(string login, string password)
    {
        var connectionString = string.Format(_pgConnTemplate, login, password);

        var connection = new NpgsqlConnection(connectionString);

        try
        {
            await connection.OpenAsync();
        }
        catch (Exception e)
        {
            return ServiceResult<bool>.BadRequest(e.Message);
        }

        if (_connections.ContainsKey(login))
        {
            return ServiceResult<bool>.BadRequest("Підключення до БД вже встановлене для даного користувача");
        }

        _connections.TryAdd(login, new DbConnectionInfo(connection));

        return ServiceResult<bool>.Ok(true);
    }

    public Task<ServiceResult<DbConnection>> GetConnection(string login)
    {
        if (!_connections.TryGetValue(login, out var connection))
        {
            return Task.FromResult(
                ServiceResult<DbConnection>.BadRequest(
                    "Невдалось отримати підключення до БД. Спробуйте перезайти в аккаунт"));
        }

        connection.CurrentlyInUse = true;
        connection.LastAccessed = DateTime.Now;
        return Task.FromResult(ServiceResult<DbConnection>.Ok(connection.Connection));
    }

    public async Task<DbConnection> GetConnectionOrThrow()
    {
        var login = _httpContextAccessor.HttpContext!.User.GetDbUserLogin();
        var serviceResult = await GetConnection(login);
        if (!serviceResult.Success) throw new InvalidOperationException("Невдалось отримати підключення до БД. Спробуйте перезайти в аккаунт");
        return serviceResult.ResponseData!;
    }

    public async Task<ServiceResult<bool>> CloseConnection(string login)
    {
        if (!_connections.TryGetValue(login, out var connection))
        {
            return ServiceResult<bool>.BadRequest("Не вдалось отримати та закрити підключення до БД");
        }

        connection.CurrentlyInUse = false;
        await connection.Connection.CloseAsync();
        _connections.Remove(login, out _);
        return ServiceResult<bool>.Ok(true);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();

        GC.SuppressFinalize(this);
    }

    private async ValueTask DisposeAsyncCore()
    {
        if (_disposed)
            return;

        foreach (var (_, dbConnectionInfo) in _connections)
        {
            await dbConnectionInfo.Connection.CloseAsync();
        }

        _disposed = true;
    }

    ~PostgresDbManager()
    {
        DisposeAsyncCore().AsTask().GetAwaiter().GetResult();
    }
}