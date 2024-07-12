using DatingAgencyMS.Client.Models.Core;

namespace DatingAgencyMS.Client.Models.DTOs.Auth;

public record LoginResponse(string Token, string Login, DbRoles Role)
{
    public LoggedInUser ToUser() => new()
    {
        Login = Login,
        Role = Role,
        Token = Token
    };
};

