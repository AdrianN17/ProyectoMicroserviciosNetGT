using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WalletService.Application.Abstractions.Secrets;
using WalletService.Infrastructure.Messaging;

namespace WalletService.Infrastructure.Configuration;

public static class ServiceBusManagerConfigurationExtension
{
    public static IServiceCollection AddServiceBusConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var serviceBusOptions = configuration.GetSection(ServiceBusOptions.SectionName).Get<ServiceBusOptions>() ?? new ServiceBusOptions();
        if (string.IsNullOrEmpty(serviceBusOptions.QueueName)) throw new InvalidOperationException("ServiceBus QueueName is not configured.");

        services.AddSingleton(serviceBusOptions);

        services.AddMassTransit(busConfig =>
        {
            busConfig.AddConsumer<UpdateBalanceConsumer>();

            busConfig.UsingAzureServiceBus((context, cfg) =>
            {
                var secretProvider = context.GetRequiredService<ISecretProvider>();
                var secretName = configuration.GetValue<string>("ServiceBusConnectionString")
                                 ?? throw new InvalidOperationException("Falta la configuración 'ServiceBusConnectionString'.");

                var connectionString = secretProvider.GetSecretAsync(secretName).GetAwaiter().GetResult()
                                       ?? throw new InvalidOperationException($"El secreto '{secretName}' no fue encontrado en KeyVault.");

                cfg.Host(connectionString);

                cfg.ReceiveEndpoint(serviceBusOptions.QueueName, (IServiceBusReceiveEndpointConfigurator e) =>
                {
                    // Azure Service Bus Basic tier only supports Queues, not Topics/Subscriptions.
                    // Disable topology configuration to prevent MassTransit from trying to create Topics.
                    e.ConfigureConsumeTopology = false;

                    // Basic tier does not support AutoDeleteOnIdle on queues (including dead-letter queues).
                    // Set to TimeSpan.MaxValue to effectively disable auto-delete.
                    e.AutoDeleteOnIdle = TimeSpan.MaxValue;

                    // Basic tier does not support creating '_skipped' or '_error' queues with AutoDeleteOnIdle.
                    // Discard skipped/faulted messages instead of forwarding them to auxiliary queues.
                    e.DiscardSkippedMessages();
                    e.DiscardFaultedMessages();

                    e.ConfigureConsumer<UpdateBalanceConsumer>(context);
                });
            });
        });


        return services;
    }
}