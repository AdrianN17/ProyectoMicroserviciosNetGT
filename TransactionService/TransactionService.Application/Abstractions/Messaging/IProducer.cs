namespace TransactionService.Application.Abstractions.Messaging;

public interface IProducer
{
    Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class;
}

