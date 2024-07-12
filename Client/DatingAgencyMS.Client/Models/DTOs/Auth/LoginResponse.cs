using DatingAgencyMS.Client.Models.Core;

namespace DatingAgencyMS.Client.Models.DTOs.Auth;

public record LoginResponse(string Token, string Login, DbRoles Role)
{
    public User ToUser() => new()
    {
        Login = Login,
        Role = Role,
        Token = Token
    };
};

