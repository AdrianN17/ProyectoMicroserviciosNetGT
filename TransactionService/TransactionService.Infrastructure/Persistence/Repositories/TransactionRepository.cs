using Microsoft.Azure.Cosmos;
using TransactionService.Domain.Interfaces;
using TransactionService.Infrastructure.Persistence.Contexts;
using TransactionService.Infrastructure.Persistence.Documents;

namespace TransactionService.Infrastructure.Persistence.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly Container _container;
        private readonly CosmosDbContext _cosmosDbContext;

        public TransactionRepository(CosmosDbContext cosmosDbContext)
        {
            _cosmosDbContext = cosmosDbContext;
            _container       = cosmosDbContext.GetContainer(CosmosDbContext.TransactionsContainer);
        }

        public async Task CreateAsync(Transaction transaction)
        {
            var document   = TransactionDocument.FromDomain(transaction);
            var partitionKey = new PartitionKey(document.FromWalletId);

            await _container.CreateItemAsync(document, partitionKey);
            await _cosmosDbContext.DispatchDomainEventsAsync(transaction);
        }

        public async Task<Transaction?> GetByIdAsync(TransactionId id, CancellationToken cancellationToken = default)
        {
            // Necesitamos el fromWalletId para la partition key; lo leemos con una query cross-partition mínima
            var query = new QueryDefinition(
                "SELECT * FROM c WHERE c.id = @id AND (c.isDeleted = false OR NOT IS_DEFINED(c.isDeleted))")
                .WithParameter("@id", id.ToString());

            var iterator = _container.GetItemQueryIterator<TransactionDocument>(
                query,
                requestOptions: new QueryRequestOptions { MaxItemCount = 1 });

            while (iterator.HasMoreResults)
            {
                var page = await iterator.ReadNextAsync(cancellationToken);
                var doc  = page.FirstOrDefault();
                if (doc is not null) return doc.ToDomain();
            }

            return null;
        }

        public async Task UpdateAsync(Transaction transaction, CancellationToken cancellationToken = default)
        {
            var document     = TransactionDocument.FromDomain(transaction);
            var partitionKey = new PartitionKey(document.FromWalletId);

            await _container.UpsertItemAsync(document, partitionKey, cancellationToken: cancellationToken);
            await _cosmosDbContext.DispatchDomainEventsAsync(transaction, cancellationToken);
        }
    }
}
