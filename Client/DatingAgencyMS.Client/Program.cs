using DatingAgencyMS.Client.Components;
using DatingAgencyMS.Client.Services;
using Refit;

var builder = WebApplication.CreateBuilder(args);
var cfg = builder.Configuration;
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddRefitClient<IDbAccessService>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(cfg["ApiBaseUrl"] ??
                                throw new ArgumentException(
                                    "Api base url was not found in configuration",
                                    "ApiBaseUrl"));
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();