using Common.Auth;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Domain.Models.DbManagement;
using DatingAgencyMS.Infrastructure.Constants;
using DatingAgencyMS.Infrastructure.Services;
using DatingAgencyMS.Infrastructure.Services.PostgresServices;
using DatingAgencyMS.Infrastructure.Settings;
using Microsoft.AspNetCore.Http;
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

        services.AddHttpContextAccessor();
        
        services.AddSingleton<IDbManager, PostgresDbManager>(sp =>
        {
            var pgConnTemplate = configuration.GetConnectionString("pg_conn_template") ??
                                 throw new ArgumentException("pg_conn_template");
            var pgRootConn = configuration.GetConnectionString("ConnectionStringForRoot") ??
                             throw new ArgumentException("ConnectionStringForRoot");
            var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
            
            return new PostgresDbManager(pgConnTemplate, pgRootConn, httpContextAccessor);
        });

        services.AddScoped<IUserManager, PostgresUserManager>();
        services.AddScoped<IClientsService, PostgresClientsService>();
        services.AddScoped<IPartnerRequirementsService, PostgresPartnerRequirementsService>();
        services.AddScoped<IInvitationsService, PostgresInvitationsService>();
        services.AddScoped<IMeetingsService, PostgresMeetingsService>();
        services.AddScoped<IVisitsService, PostgresVisitsService>();
        services.AddScoped<ICoupleArchiveService, PostgresCoupleArchiveService>();
        services.AddScoped<IAdditionalContactsService, PostgresAdditionalContactsService>();
        
        return services;
    }
}