﻿using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TransactionService.Application.Abstractions.Messaging;
using TransactionService.Application.Abstractions.Secrets;
using TransactionService.Infrastructure.Messaging;

namespace TransactionService.Infrastructure.Configuration;

public static class ServiceBusManagerConfigurationExtension
{
    public static IServiceCollection AddServiceBusConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var serviceBusOptions = configuration.GetSection(ServiceBusOptions.SectionName).Get<ServiceBusOptions>() ?? new ServiceBusOptions();
        if (string.IsNullOrEmpty(serviceBusOptions.QueueName)) throw new InvalidOperationException("ServiceBus QueueName is not configured.");

        services.AddSingleton(serviceBusOptions);

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
            });
        });

        // Producer es el adaptador (patrón Adapter) entre la abstracción IProducer de Application
        // y MassTransit. La capa Application no conoce MassTransit en ningún momento.
        services.AddScoped<IProducer, Producer>();

        return services;
    }
}

