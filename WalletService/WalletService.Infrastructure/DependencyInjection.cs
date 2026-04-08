using MassTransit;
using WalletService.Application.Abstractions.Secrets;
using WalletService.Application.Commmon.Interfaces;
using WalletService.Domain.Interfaces;
using WalletService.Infrastructure.Caching;
using WalletService.Infrastructure.Configuration;
using WalletService.Infrastructure.Persistence.Contexts;
using WalletService.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WalletService.Infrastructure.Messaging;
using IConsumer = WalletService.Application.Commmon.Interfaces.IConsumer;

namespace WalletService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var secretProviderType = configuration.GetValue<string>("SecretProviderType")?.ToLower();
            if (string.IsNullOrEmpty(secretProviderType)) throw new InvalidOperationException("SecretProviderType configuration is missing. Valid values are 'SecretsManager' or 'Vault'.");

            if (secretProviderType.Equals("keyvault", StringComparison.CurrentCultureIgnoreCase))
            {
                services.AddKeyVaultConfiguration(configuration);
            }
            else
            {
                throw new InvalidOperationException("Invalid SecretProviderType configuration. Valid values are 'SecretsManager' or 'Vault'.");
            }
            services.AddSingleton<InMemorySecretCache>();
            services.AddPersistence(configuration);
            services.AddServiceBus(configuration);

            return services;
        }
        
        private static IServiceCollection AddServiceBus(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(busConfig =>
            {
                busConfig.UsingAzureServiceBus((context, cfg) =>
                {
                    var secretProvider = context.GetRequiredService<ISecretProvider>();
                    var secretName = configuration.GetValue<string>("ServiceBusConnectionString")
                                     ?? throw new InvalidOperationException("Falta la configuración 'ServiceBusConnectionString'.");

                    var connectionString = secretProvider.GetSecretAsync(secretName).GetAwaiter().GetResult()
                                           ?? throw new InvalidOperationException($"El secreto '{secretName}' no fue encontrado en KeyVault.");

                    cfg.Host(connectionString);
                    cfg.ConfigureEndpoints(context);
                });
            });

            services.AddScoped<IConsumer, Consumer>();

            return services;
        }

        private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                var secrets = sp.GetRequiredService<ISecretProvider>();
                var connectionString = secrets.GetSecretAsync("WalletSqlServerConnection").GetAwaiter().GetResult();

                if (connectionString is null) {
                    throw new InvalidOperationException("Connection string for CustomerSqlServerConnection is not configured in Vault");
                }

                options.UseSqlServer(connectionString);
            });

            services.AddScoped<IWalletRepository, WalletRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;    
        }
    }
}
