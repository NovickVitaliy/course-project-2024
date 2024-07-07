using System.Text;
using Common.Auth;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Infrastructure.Services;
using DatingAgencyMS.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DatingAgencyMS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<JwtSettings>()
            .BindConfiguration(JwtSettings.ConfigPath)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.ConfigureAuth(configuration);

        services.AddSingleton<ITokenService, DefaultTokenService>();

        return services;
    }
}