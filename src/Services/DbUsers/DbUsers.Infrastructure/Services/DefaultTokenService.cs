using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DbUsers.Application.Contracts;
using DbUsers.Infrastructure.Constants;
using DbUsers.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DbUsers.Infrastructure.Services;

public class DefaultTokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;

    public DefaultTokenService(IOptions<JwtSettings> options)
    {
        _jwtSettings = options.Value;
    }

    public Task<string> GenerateJwtToken(string login)
    {
        //TODO: retrieve role for the user and put in claims

        var now = DateTimeOffset.Now;
        var expiresAt = now.AddMinutes(_jwtSettings.LifeTimeInMinutes);

        Claim[] claims =
        [
            new Claim(JwtRegisteredClaimNames.Exp, expiresAt.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new Claim(ApplicationClaimTypes.DbUser, login)
        ];

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
            SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            signingCredentials: credentials
        );

        return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken));
    }
}