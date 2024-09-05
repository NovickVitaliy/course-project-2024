using DbUp;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace DatingAgencyMS.Infrastructure.DbSetup;

public static class DbSetup
{
    public static Task MigrateDatabase(this WebApplication app)
    {
        var pgRootConnection = app.Configuration.GetConnectionString("ConnectionStringForRoot");
        
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