using Microsoft.Azure.Cosmos;
using TransactionService.Domain.Entities;
using TransactionService.Domain.Interfaces;
using TransactionService.Domain.ValueObjects;
using TransactionService.Infrastructure.Persistence.Contexts;
using TransactionService.Infrastructure.Persistence.Documents;

namespace TransactionService.Infrastructure.Persistence.Repositories
{
    public class RechargeRepository : IRechargeRepository
    {
        private readonly Container _container;
        private readonly CosmosDbContext _cosmosDbContext;

        public RechargeRepository(CosmosDbContext cosmosDbContext)
        {
            _cosmosDbContext = cosmosDbContext;
            _container       = cosmosDbContext.GetContainer(CosmosDbContext.RechargesContainer);
        }

        public async Task CreateAsync(Recharge recharge)
        {
            var document     = RechargeDocument.FromDomain(recharge);
            var partitionKey = new PartitionKey(document.WalletId);

            await _container.CreateItemAsync(document, partitionKey);
            await _cosmosDbContext.DispatchDomainEventsAsync(recharge);
        }

        public async Task<Recharge?> GetByIdAsync(RechargeId id, CancellationToken cancellationToken = default)
        {
            var query = new QueryDefinition(
                "SELECT * FROM c WHERE c.id = @id AND (c.isDeleted = false OR NOT IS_DEFINED(c.isDeleted))")
                .WithParameter("@id", id.ToString());

            var iterator = _container.GetItemQueryIterator<RechargeDocument>(
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

        public async Task UpdateAsync(Recharge recharge, CancellationToken cancellationToken = default)
        {
            var document     = RechargeDocument.FromDomain(recharge);
            var partitionKey = new PartitionKey(document.WalletId);

            await _container.UpsertItemAsync(document, partitionKey, cancellationToken: cancellationToken);
            await _cosmosDbContext.DispatchDomainEventsAsync(recharge, cancellationToken);
        }
    }
}
