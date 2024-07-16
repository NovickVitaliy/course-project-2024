using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Net;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.Shared;
using DatingAgencyMS.Infrastructure.Models;
using Npgsql;

namespace DatingAgencyMS.Infrastructure.Services;

public class PostgresDbManager : IDbManager, IAsyncDisposable
{
    private bool _disposed = false;
    private readonly string _pgConnTemplate;
    private readonly ConcurrentDictionary<string, DbConnectionInfo> _connections;
    private readonly DbConnection _rootConnection;
    public PostgresDbManager(string pgConnTemplate, string pgRootConn)
    {
        _pgConnTemplate = pgConnTemplate;
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
            return new ServiceResult<bool>(false, (int)HttpStatusCode.BadRequest, false, e.Message);
        }

        if (_connections.ContainsKey(login))
        {
            return new ServiceResult<bool>(false, (int)HttpStatusCode.BadRequest, false,
                "Підключення до БД вже встановлене для даного користувача");
        }

        _connections.TryAdd(login, new DbConnectionInfo(connection));

        return new ServiceResult<bool>(true, (int)HttpStatusCode.OK, true);
    }

    public Task<ServiceResult<DbConnection>> GetConnection(string login)
    {
        if (!_connections.TryGetValue(login, out var connection))
        {
            return Task.FromResult(new ServiceResult<DbConnection>(false, (int)HttpStatusCode.BadRequest, null,
                "Невдалось отримати підключення до БД. Спробуйте перезайти в аккаунт"));
        }

        connection.CurrentlyInUse = true;
        connection.LastAccessed = DateTime.Now;
        return Task.FromResult(new ServiceResult<DbConnection>(true, (int)HttpStatusCode.OK, connection.Connection));
    }

    public async Task<ServiceResult<bool>> CloseConnection(string login)
    {
        if (!_connections.TryGetValue(login, out var connection))
        {
            return new ServiceResult<bool>(false, (int)HttpStatusCode.BadRequest, false,
                "Не вдалось отримати та закрити підключення до БД");
        }

        connection.CurrentlyInUse = false;
        await connection.Connection.CloseAsync();
        _connections.Remove(login, out _);
        return new ServiceResult<bool>(true, (int)HttpStatusCode.OK, true);
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