﻿﻿using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;
using Polly.Retry;
using Polly.Timeout;
using TransactionService.Application.Abstractions.Messaging;
using TransactionService.Application.Abstractions.Secrets;
using TransactionService.Application.Abstractions.Services;
using TransactionService.Application.Commmon.Interfaces;
using TransactionService.Domain.Interfaces;
using TransactionService.Infrastructure.Caching;
using TransactionService.Infrastructure.Configuration;
using TransactionService.Infrastructure.Messaging;
using TransactionService.Infrastructure.Persistence.Contexts;
using TransactionService.Infrastructure.Persistence.Repositories;
using TransactionService.Infrastructure.Services;

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

            services.AddScoped<IProducer, Producer>();

            return services;
        }

        private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                var secrets      = sp.GetRequiredService<ISecretProvider>();
                var endpoint     = secrets.GetSecretAsync("CosmosEndpoint").GetAwaiter().GetResult()
                                   ?? throw new InvalidOperationException("Secret 'CosmosEndpoint' is not configured.");
                var accountKey   = secrets.GetSecretAsync("CosmosAccountKey").GetAwaiter().GetResult()
                                   ?? throw new InvalidOperationException("Secret 'CosmosAccountKey' is not configured.");
                var databaseName = configuration.GetValue<string>("CosmosDatabaseName")
                                   ?? throw new InvalidOperationException("Secret 'CosmosDatabaseName' is not configured.");

                options.UseCosmos(endpoint, accountKey, databaseName);
            });
            
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IRechargeRepository,    RechargeRepository>();
            services.AddScoped<IUnitOfWork,            UnitOfWork>();

            var timeoutPolicy = GetTimeoutPolicy();
            var retryPolicy = GetRetryPolicy();
            var circuitBreakerPolicy = GetCircuitBreakerPolicy();
            services.AddHttpClient<IWalletReadService, WalletReadService>(client =>
                {
                    var url = configuration.GetSection("Services:WalletService").Value;
                    if (string.IsNullOrWhiteSpace(url))
                        throw new InvalidOperationException("Falta la configuración 'Services:WalletService'.");
                    client.BaseAddress = new Uri(url);

                })
                .AddPolicyHandler(retryPolicy)
                .AddPolicyHandler(circuitBreakerPolicy)
                .AddPolicyHandler(timeoutPolicy);
            
            services.AddHttpClient<IWalletReadService, WalletReadService>(client =>
                {
                    var url = configuration.GetSection("Services:WalletService").Value;
                    if (string.IsNullOrWhiteSpace(url))
                        throw new InvalidOperationException("Falta la configuración 'Services:WalletService'.");
                    client.BaseAddress = new Uri(url);

                })
                .AddPolicyHandler(retryPolicy)
                .AddPolicyHandler(circuitBreakerPolicy)
                .AddPolicyHandler(timeoutPolicy);
            
            return services;
        }
        
        private static AsyncTimeoutPolicy<HttpResponseMessage> GetTimeoutPolicy()
        {
            return Policy.TimeoutAsync<HttpResponseMessage>(
                seconds: 5,
                timeoutStrategy: TimeoutStrategy.Optimistic, 
                (context, timespan, task) =>
                {
                    Console.WriteLine($"Timeout alcanzado despues de {timespan.TotalSeconds}");
                    return Task.CompletedTask;
                }
            ); 
        }

        private static AsyncRetryPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(5, retryAttemp => TimeSpan.FromSeconds(Math.Pow(2, retryAttemp)),
                    onRetry: (outcome, timespan, retryAttemp, context) =>
                    {
                        Console.WriteLine($"Reintento {retryAttemp} despues de {timespan.TotalSeconds} segundos {outcome.Exception?.Message}");
                    }
                );
        }

        private static AsyncCircuitBreakerPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 3, 
                    durationOfBreak: TimeSpan.FromSeconds(30),
                    onBreak: (outcome, timespan) =>
                    {
                        Console.WriteLine($"Circuito abierto por {timespan.TotalSeconds} debido a {outcome.Exception?.Message}");
                    },
                    onHalfOpen: () =>
                    {
                        Console.WriteLine($"Circuito semi abierto");
                    },
                    onReset: () =>
                    {
                        Console.WriteLine($"Circuito cerrado - Esta trabajando con normalidad");
                    }
                );
        }
    }
}
