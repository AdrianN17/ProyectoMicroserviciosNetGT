using MassTransit;
using Microsoft.Extensions.Logging;
using TransactionService.Application.Abstractions.Messaging;

namespace TransactionService.Infrastructure.Messaging;

/// <summary>
/// Adaptador (patrón Adapter) que implementa la abstracción IProducer de la capa Application
/// usando MassTransit como mecanismo de transporte. La capa Application no conoce MassTransit.
/// </summary>
public sealed class Producer(IPublishEndpoint publishEndpoint, ILogger<Producer> logger) : IProducer
{
    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
    {
        logger.LogInformation("Publishing message {MessageType} to Service Bus", typeof(T).Name);
        await publishEndpoint.Publish(message, cancellationToken);
        logger.LogInformation("Message {MessageType} published successfully", typeof(T).Name);
    }
}

