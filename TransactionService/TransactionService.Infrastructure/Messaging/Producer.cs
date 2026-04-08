using MassTransit;
using Microsoft.Extensions.Logging;
using TransactionService.Application.Abstractions.Messaging;

namespace TransactionService.Infrastructure.Messaging;

public sealed class Producer : IProducer
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<Producer> _logger;

    public Producer(IPublishEndpoint publishEndpoint, ILogger<Producer> logger)
    {
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
    {
        _logger.LogInformation("Publishing message {MessageType} to Service Bus", typeof(T).Name);
        await _publishEndpoint.Publish(message, cancellationToken);
        _logger.LogInformation("Message {MessageType} published successfully", typeof(T).Name);
    }
}

