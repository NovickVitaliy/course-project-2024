using DatingAgencyMS.API.Controllers.Base;
using DatingAgencyMS.Application.DTOs.UserManagement;
using DatingAgencyMS.Infrastructure.Helpers;
using DatingAgencyMS.Infrastructure.SqlQueryBuilder.Common;
using DatingAgencyMS.Infrastructure.SqlQueryBuilder.Keys;
using Microsoft.AspNetCore.Authorization;
using Npgsql;

namespace DatingAgencyMS.API.Controllers;

public class TestingController : BaseApiController
{
    private readonly string _connectionStringTemplate;
    public TestingController([FromKeyedServices("pg_conn_template")]string connectionStringTemplate)
    {
        _connectionStringTemplate = connectionStringTemplate;
    }
    
    [HttpGet]
    public async Task<IActionResult> PasswordHasher(string login, string password)
    {
        await using var connection = new NpgsqlConnection(string.Format(_connectionStringTemplate, "admin", "xc56-426i-rkmf"));
        await connection.OpenAsync();
        await using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT * FROM keys WHERE login='admin'";
        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();
        var hash = reader.GetString(reader.GetOrdinal("password_hash"));
        var salt = reader.GetString(reader.GetOrdinal("password_salt"));

        var equals = PasswordHelper.VerifyPassword(password, hash, salt);

        return Ok(new {equals});
    }

    
}