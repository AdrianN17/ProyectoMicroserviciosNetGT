using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WalletService.Application.Abstractions.Secrets;
using WalletService.Application.Wallets.Commands.UpdateBalance;
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

                cfg.Message<UpdateBalanceCommand>(x => x.SetEntityName(serviceBusOptions.QueueName));

                cfg.ConfigureEndpoints(context);
            });
        });


        return services;
    }
}