using MediatR;
using Microsoft.Azure.Cosmos;
using TransactionService.Infrastructure.Persistence.Contexts;

namespace TransactionService.Infrastructure.Persistence.Contexts
{
    /// <summary>
    /// Inicializa la base de datos y los containers de Cosmos DB si no existen.
    /// Úsala al arrancar la aplicación (o en scripts de despliegue).
    /// </summary>
    public static class CosmosDbInitializer
    {
        public static async Task EnsureCreatedAsync(CosmosClient client, string databaseName)
        {
            // Crear la base de datos si no existe
            var dbResponse = await client.CreateDatabaseIfNotExistsAsync(databaseName);
            var database   = dbResponse.Database;

            // Crear container de Transactions  (partition key = /fromWalletId)
            await database.CreateContainerIfNotExistsAsync(
                new ContainerProperties(CosmosDbContext.TransactionsContainer, "/fromWalletId"));

            // Crear container de Recharges (partition key = /walletId)
            await database.CreateContainerIfNotExistsAsync(
                new ContainerProperties(CosmosDbContext.RechargesContainer, "/walletId"));
        }
    }

    // Mantener el NoOpPublisher por si se necesita en tests
    public sealed class NoOpPublisher : IPublisher
    {
        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification => Task.CompletedTask;

        public Task Publish(object notification, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }
}
