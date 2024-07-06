using DbUsers.Application.Contracts;
using DbUsers.Infrastructure.Services;

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

var app = builder.Build();

app.MapControllers();

app.Run();
