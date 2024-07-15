using Blazored.LocalStorage;
using DatingAgencyMS.Client.Components;
using DatingAgencyMS.Client.Extensions;
using DatingAgencyMS.Client.Services;
using Fluxor;

var builder = WebApplication.CreateBuilder(args);
var cfg = builder.Configuration;
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services
    .AddRefitServiceWithBaseApiUrl<IDbAccessService>(cfg)
    .AddRefitServiceWithBaseApiUrl<IUsersService>(cfg);

builder.Services.AddFluxor(options =>
{
    options.ScanAssemblies(typeof(Program).Assembly);
});

builder.Services.AddBlazorBootstrap();
builder.Services.AddBlazoredLocalStorage();

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