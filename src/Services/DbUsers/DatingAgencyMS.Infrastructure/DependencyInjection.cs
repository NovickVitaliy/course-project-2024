using Common.Auth;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Domain.Models;
using DatingAgencyMS.Domain.Models.DbManagement;
using DatingAgencyMS.Infrastructure.Constants;
using DatingAgencyMS.Infrastructure.Services;
using DatingAgencyMS.Infrastructure.Services.PostgresServices;
using DatingAgencyMS.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
        services.AddAuthorizationBuilder()
            .AddPolicy(ApplicationPolicies.IsOwnerOrAdmin, builder =>
            {
                builder.RequireClaim(ApplicationClaimTypes.DbRole,
                    [DbRoles.Owner.ToString(), DbRoles.Admin.ToString()]);
            })
            .AddPolicy(ApplicationPolicies.CreateUpdateDeleteAccess,
                builder =>
                {
                    builder.RequireClaim(ApplicationClaimTypes.DbRole,
                        [DbRoles.Owner.ToString(), DbRoles.Admin.ToString(), DbRoles.Operator.ToString()]);
                });

        services.AddScoped<ITokenService, DefaultTokenService>();

        services.AddSingleton<IDbManager, PostgresDbManager>(_ => new PostgresDbManager(
            pgConnTemplate: configuration.GetConnectionString("pg_conn_template") ??
                            throw new ArgumentException("pg_conn_template"),
            pgRootConn: configuration.GetConnectionString("ConnectionStringForRoot") ??
                        throw new ArgumentException("ConnectionStringForRoot")));

        services.AddScoped<IUserManager, PostgresUserManager>();
        services.AddScoped<IClientsService, PostgresClientsService>();
        services.AddScoped<IPartnerRequirementsService, PostgresPartnerRequirementsService>();
        services.AddScoped<IInvitationsService, PostgresInvitationsService>();
        return services;
    }
}