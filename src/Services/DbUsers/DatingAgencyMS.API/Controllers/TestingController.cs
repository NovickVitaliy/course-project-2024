using DatingAgencyMS.API.Controllers.Base;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Application.DTOs.UserManagement;
using DatingAgencyMS.Infrastructure.Helpers;
using Npgsql;

namespace DatingAgencyMS.API.Controllers;

public class TestingController : BaseApiController
{
    private readonly string _connectionStringTemplate;
    private readonly IUserManager _userManager;
    public TestingController([FromKeyedServices("pg_conn_template")]string connectionStringTemplate, IUserManager userManager)
    {
        _connectionStringTemplate = connectionStringTemplate;
        _userManager = userManager;
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

    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserRequest request)
    {
        var result = await _userManager.CreateUser(request);

        return Ok(new {result.Code, result.Description});
    }
}