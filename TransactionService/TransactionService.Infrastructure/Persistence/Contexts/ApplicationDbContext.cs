﻿
using MediatR;
using Microsoft.Azure.Cosmos;
using TransactionService.Domain.Common;

namespace TransactionService.Infrastructure.Persistence.Contexts
{
    public class CosmosDbContext
    {
        private readonly CosmosClient _client;
        private readonly IPublisher _publisher;
        private readonly string _databaseName;

        public const string TransactionsContainer = "Transactions";
        public const string RechargesContainer    = "Recharges";

        public CosmosDbContext(CosmosClient client, string databaseName, IPublisher publisher)
        {
            _client       = client;
            _databaseName = databaseName;
            _publisher    = publisher;
        }

        public Container GetContainer(string containerName)
            => _client.GetContainer(_databaseName, containerName);

        /// <summary>
        /// Publica los domain events acumulados en el aggregate y los limpia.
        /// Llamar después de cada operación de escritura.
        /// </summary>
        public async Task DispatchDomainEventsAsync<TId>(
            AggregateRoot<TId> aggregate,
            CancellationToken cancellationToken = default)
        {
            var events = aggregate.DomainEvents.ToList();
            aggregate.ClearDomainEvents();

            foreach (var domainEvent in events)
                await _publisher.Publish(domainEvent, cancellationToken);
        }
    }
}
