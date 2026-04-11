using Azure.Identity;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Scalar.AspNetCore;
using TransactionService.Api;
using TransactionService.Application;
using TransactionService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ── Azure App Configuration ──────────────────────────────────────────────────
var appConfigEndpoint = builder.Configuration["AzureAppConfiguration:Endpoint"]
                        ?? throw new InvalidOperationException("Falta 'AzureAppConfiguration:Endpoint' en appsettings.json");

builder.Configuration.AddAzureAppConfiguration(options =>
{
    options
        .Connect(new Uri(appConfigEndpoint), new DefaultAzureCredential())
        // Carga todas las claves (sin prefijo o ajusta el selector según tu convención)
        .Select(KeyFilter.Any)
        // Resuelve Key Vault references automáticamente con la misma identidad
        .ConfigureKeyVault(kv =>
        {
            kv.SetCredential(new DefaultAzureCredential());
        });
});

builder.Services.AddAzureAppConfiguration();
// ────────────────────────────────────────────────────────────────────────────

// Add services to the container.

builder.Services.AddPresentation();

builder.Services.AddApplication();

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); // scalar/v1
}
else
{
    app.UseHsts();
}

app.UseAzureAppConfiguration();

// ✅ Esto hace que NO salga el mega detalle del DeveloperExceptionPage
app.UseExceptionHandler(/*new ExceptionHandlerOptions { SuppressDiagnosticsCallback = _ => false }*/);;

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();