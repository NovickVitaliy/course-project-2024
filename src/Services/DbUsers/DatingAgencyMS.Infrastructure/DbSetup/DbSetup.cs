using DatingAgencyMS.Infrastructure.Helpers;
using DbUp;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace DatingAgencyMS.Infrastructure.DbSetup;

public static class DbSetup
{
    public static Task MigrateDatabase(this WebApplication app)
    {
        var pgRootConnection = app.Services.GetRequiredKeyedService<string>("pg_root_conn");
        
        EnsureDatabase.For.PostgresqlDatabase(pgRootConnection);

        var upgrader = DeployChanges.To.PostgresqlDatabase(pgRootConnection)
            .WithScriptsEmbeddedInAssembly(typeof(DbSetup).Assembly)
            .LogToConsole()
            .Build();

        if (upgrader.IsUpgradeRequired())
        {
            upgrader.PerformUpgrade();
        }

        return Task.CompletedTask;
    }
}