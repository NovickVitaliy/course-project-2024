using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Domain.Models.DbManagement;
using DatingAgencyMS.Infrastructure.Constants;
using DatingAgencyMS.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DatingAgencyMS.Infrastructure.Services;

public class DefaultTokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;

    public DefaultTokenService(IOptions<JwtSettings> options, IUserManager userManager)
    {
        _jwtSettings = options.Value;
    }

    public Task<string> GenerateJwtToken(string login, DbRoles role)
    {
        var now = DateTimeOffset.UtcNow;
        var expiresAt = now.AddMinutes(_jwtSettings.LifeTimeInMinutes);

        Claim[] claims =
        [
            new Claim(JwtRegisteredClaimNames.Exp, expiresAt.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new Claim(ApplicationClaimTypes.DbUser, login),
            new Claim(ApplicationClaimTypes.DbRole, role.ToString())
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