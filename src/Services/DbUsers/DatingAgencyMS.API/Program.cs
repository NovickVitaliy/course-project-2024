using System.Text.Json.Serialization;
using DatingAgencyMS.Application.Contracts;
using DatingAgencyMS.Infrastructure;
using DatingAgencyMS.Infrastructure.DbSetup;
using DatingAgencyMS.Infrastructure.Services;

[assembly: ApiController]

var builder = WebApplication.CreateBuilder(args);
var cfg = builder.Configuration;

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options => { options.SuppressModelStateInvalidFilter = true; })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddKeyedSingleton("pg_conn_template",
    cfg.GetConnectionString("pg_conn_template") ?? throw new ArgumentException("pg_conn_template"));
builder.Services.AddKeyedSingleton("pg_root_conn",
    cfg.GetConnectionString("ConnectionStringForRoot") ?? throw new ArgumentException("pg_root_conn"));

builder.Services.ConfigureInfrastructure(cfg);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await app.MigrateDatabase();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();