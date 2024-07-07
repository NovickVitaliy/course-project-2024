using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Infrastructure;
using DatingAgencyMS.Infrastructure.Services;

[assembly:ApiController]

var builder = WebApplication.CreateBuilder(args);
var cfg = builder.Configuration;

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

builder.Services.AddSingleton<IDbManager, PostgresDbManager>();

builder.Services.AddKeyedSingleton<string>("pg_conn_template",
    cfg["pg_conn_template"] ?? throw new ArgumentException("pg_conn_template"));

builder.Services.ConfigureInfrastructure(cfg);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
