using System.Text.Json.Serialization;
using DatingAgencyMS.Application.Options;
using DatingAgencyMS.Infrastructure;
using DatingAgencyMS.Infrastructure.DbSetup;

[assembly: ApiController]

var builder = WebApplication.CreateBuilder(args);
var cfg = builder.Configuration;

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options => { options.SuppressModelStateInvalidFilter = true; })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddOptions<PasswordEncryptionOptions>()
    .BindConfiguration(PasswordEncryptionOptions.Position)
    .ValidateOnStart()
    .ValidateDataAnnotations();

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