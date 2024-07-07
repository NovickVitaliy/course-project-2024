using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace DatingAgencyMS.Infrastructure.DbSetup;

public static class DbSetup
{
    public static async Task SetupInitialDbWithUsersAndRole(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var pgConnTemplate = scope.ServiceProvider.GetRequiredKeyedService<string>("pg_conn_template");
        var connectionString = string.Format(pgConnTemplate, "admin", "xc56-426i-rkmf");

        //TODO: change in the future to be scalable
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        await using var cmd = connection.CreateCommand();

        await CreateInitialTables(cmd);
        await SeedTablesWithInitialData(cmd);
    }

    private static async Task SeedTablesWithInitialData(NpgsqlCommand cmd)
    {
        await SeedInitialRoles(cmd);
        await SeedInitialUsers(cmd);
    }

    private static async Task SeedInitialUsers(NpgsqlCommand cmd)
    {
        //TODO: seed initial user that is admin
    }

    private static async Task SeedInitialRoles(NpgsqlCommand cmd)
    {
        cmd.CommandText = "INSERT INTO roles (name) VALUES " +
                          "('owner'), " +
                          "('admin'), " +
                          "('operator'), " +
                          "('guest')";
        
        await cmd.ExecuteNonQueryAsync();
    }

    private static async Task CreateInitialTables(NpgsqlCommand cmd)
    {
        await CreateRolesTable(cmd);
        await CreateKeysTable(cmd);
    }

    private static async Task CreateRolesTable(NpgsqlCommand cmd)
    {
        cmd.CommandText = "CREATE TABLE IF NOT EXISTS Roles(" +
                          "id SERIAL," +
                          "name VARCHAR(50)," +
                          "PRIMARY KEY (id));";
        
        await cmd.ExecuteNonQueryAsync();
    }

    private static async Task CreateKeysTable(NpgsqlCommand cmd)
    {
        cmd.CommandText = "CREATE TABLE IF NOT EXISTS Keys (" +
                          "id SERIAL, " +
                          "login VARCHAR(50) NOT NULL," +
                          "password_hash VARCHAR(64) NOT NULL," +
                          "password_salt VARCHAR(64) NOT NULL," +
                          "role_id INT NOT NULL," +
                          "PRIMARY KEY(id)," +
                          "FOREIGN KEY (role_id) REFERENCES Roles(id) ON DELETE CASCADE);";
        
        await cmd.ExecuteNonQueryAsync();
    }
}