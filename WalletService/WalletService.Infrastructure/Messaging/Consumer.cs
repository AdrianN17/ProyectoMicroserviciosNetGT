using Microsoft.Extensions.Logging;
using IConsumer = WalletService.Application.Commmon.Interfaces.IConsumer;

namespace WalletService.Infrastructure.Messaging;

public sealed class Consumer : IConsumer
{
    private readonly ILogger<Consumer> _logger;

    public Consumer(ILogger<Consumer> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task ConsumeAsync(string rawMessage, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Message received from Service Bus: {RawMessage}", rawMessage);

        // Parsear rawMessage a Operation (dominio) aquí
        // Operation operation = ...

        return Task.CompletedTask;
    }
}