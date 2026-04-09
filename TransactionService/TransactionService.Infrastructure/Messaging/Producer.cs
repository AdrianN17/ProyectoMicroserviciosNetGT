using MassTransit;
using Microsoft.Extensions.Logging;
using TransactionService.Application.Abstractions.Messaging;
using TransactionService.Infrastructure.Configuration;

namespace TransactionService.Infrastructure.Messaging;

/// <summary>
/// Adaptador (patrón Adapter) que implementa la abstracción IProducer de la capa Application
/// usando MassTransit como mecanismo de transporte. La capa Application no conoce MassTransit.
/// </summary>
public sealed class Producer(
    ISendEndpointProvider sendEndpointProvider,
    ServiceBusOptions serviceBusOptions,
    ILogger<Producer> logger) : IProducer
{
    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
    {
        logger.LogInformation("Publishing message {MessageType} to Service Bus queue {QueueName}", typeof(T).Name, serviceBusOptions.QueueName);
        var endpoint = await sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{serviceBusOptions.QueueName}"));
        await endpoint.Send(message, cancellationToken);
        logger.LogInformation("Message {MessageType} published successfully", typeof(T).Name);
    }
}
