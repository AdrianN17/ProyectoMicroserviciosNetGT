using MediatR;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TransactionService.Application.Abstractions.Secrets;
using TransactionService.Application.Commmon.Interfaces;
using TransactionService.Domain.Interfaces;
using TransactionService.Infrastructure.Caching;
using TransactionService.Infrastructure.Configuration;
using TransactionService.Infrastructure.Persistence.Contexts;
using TransactionService.Infrastructure.Persistence.Repositories;

namespace TransactionService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var secretProviderType = configuration.GetValue<string>("SecretProviderType")?.ToLower();
            if (string.IsNullOrEmpty(secretProviderType))
                throw new InvalidOperationException("SecretProviderType configuration is missing. Valid values are 'KeyVault'.");

            if (secretProviderType.Equals("keyvault", StringComparison.CurrentCultureIgnoreCase))
            {
                services.AddKeyVaultConfiguration(configuration);
            }
            else
            {
                throw new InvalidOperationException("Invalid SecretProviderType configuration. Valid values are 'KeyVault'.");
            }

            services.AddSingleton<InMemorySecretCache>();
            services.AddPersistence(configuration);

            return services;
        }

        private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            // ── Cosmos DB ───────────────────────────────────────────────────────────
            // Los valores se resuelven en tiempo de ejecución desde Key Vault / config.
            // Se espera que el ISecretProvider esté registrado antes de llegar aquí.
            services.AddSingleton<CosmosClient>(sp =>
            {
                var secrets      = sp.GetRequiredService<ISecretProvider>();
                var endpoint     = secrets.GetSecretAsync("CosmosEndpoint").GetAwaiter().GetResult()
                                   ?? throw new InvalidOperationException("Secret 'CosmosEndpoint' is not configured.");
                var accountKey   = secrets.GetSecretAsync("CosmosAccountKey").GetAwaiter().GetResult()
                                   ?? throw new InvalidOperationException("Secret 'CosmosAccountKey' is not configured.");

                return new CosmosClient(endpoint, accountKey, new CosmosClientOptions
                {
                    SerializerOptions = new CosmosSerializationOptions
                    {
                        PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                    }
                });
            });

            services.AddScoped<CosmosDbContext>(sp =>
            {
                var client        = sp.GetRequiredService<CosmosClient>();
                var publisher     = sp.GetRequiredService<IPublisher>();
                var databaseName  = configuration.GetValue<string>("CosmosDatabaseName")
                                    ?? "TransactionServiceDb";

                return new CosmosDbContext(client, databaseName, publisher);
            });

            // ── Repositorios ────────────────────────────────────────────────────────
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IRechargeRepository,    RechargeRepository>();
            services.AddScoped<IUnitOfWork,            UnitOfWork>();

            return services;
        }
    }
}
