using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Infrastructure.Constants;
using DatingAgencyMS.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DatingAgencyMS.Infrastructure.Services;

public class DefaultTokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly IUserManager _userManager;
    public DefaultTokenService(IOptions<JwtSettings> options, IUserManager userManager)
    {
        _userManager = userManager;
        _jwtSettings = options.Value;
    }

    public async Task<string> GenerateJwtToken(string login)
    {
        var serviceResult = await _userManager.GetUserRole(login);
        var now = DateTimeOffset.Now;
        var expiresAt = now.AddMinutes(_jwtSettings.LifeTimeInMinutes);

        Claim[] claims =
        [
            new Claim(JwtRegisteredClaimNames.Exp, expiresAt.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new Claim(ApplicationClaimTypes.DbUser, login),
            new Claim(ApplicationClaimTypes.DbRole, serviceResult.ResponseData!)
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

        return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
    }
}