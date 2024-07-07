using System.Collections.Concurrent;
using System.Data.Common;
using System.Net;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.Shared;
using DatingAgencyMS.Infrastructure.Models;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace DatingAgencyMS.Infrastructure.Services;

public class PostgresDbManager : IDbManager
{
    private readonly string _pgConnTemplate;
    private readonly ConcurrentDictionary<string, DbConnectionInfo> _connections;
    private const int MsDelayToAddConnectionToPool = 100;
    private const int ExponentialDelayMultiplier = 2;

    public PostgresDbManager([FromKeyedServices("pg_conn_template")] string pgConnTemplate)
    {
        _pgConnTemplate = pgConnTemplate;
        _connections = new ConcurrentDictionary<string, DbConnectionInfo>();
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

        var connectionAddedToPool = false;
        var delay = MsDelayToAddConnectionToPool;
        while (!connectionAddedToPool)
        {
            connectionAddedToPool = _connections.TryAdd(login, new DbConnectionInfo(connection));
            await Task.Delay(delay);
            delay *= ExponentialDelayMultiplier;
        }

        return new ServiceResult<bool>(true, (int)HttpStatusCode.OK, true);
    }

    public Task<ServiceResult<DbConnection>> GetConnection(string login)
    {
        if (!_connections.TryGetValue(login, out var connection))
        {
            return Task.FromResult(new ServiceResult<DbConnection>(false, (int)HttpStatusCode.BadRequest, null,
                "Could not resolve connection to DB. Try to log in again."));
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
                "Could not resolve and close the connection.");
        }

        connection.CurrentlyInUse = false;
        await connection.Connection.CloseAsync();
        _connections.Remove(login, out _);
        return new ServiceResult<bool>(true, (int)HttpStatusCode.OK, true);
    }
}