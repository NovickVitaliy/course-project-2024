using System.Security.Claims;
using DatingAgencyMS.Infrastructure.Constants;

namespace DatingAgencyMS.Infrastructure.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetDbUserLogin(this ClaimsPrincipal claimsPrincipal)
        => claimsPrincipal.Claims.First(x => x.Type == ApplicationClaimTypes.DbUser).Value;
}