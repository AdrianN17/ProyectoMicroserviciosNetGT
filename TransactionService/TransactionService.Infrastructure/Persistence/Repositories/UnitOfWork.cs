using TransactionService.Application.Commmon.Interfaces;

namespace TransactionService.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Cosmos DB no soporta transacciones distribuidas entre containers.
    /// Las operaciones atómicas se manejan a nivel de un solo item (Transactional Batch dentro del mismo partition key).
    /// Esta implementación mantiene la interfaz IUnitOfWork por compatibilidad con la capa Application.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        // SaveChangesAsync es un no-op en Cosmos: cada repositorio persiste
        // inmediatamente al llamar CreateAsync / UpdateAsync.
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(0);

        public void Dispose() { }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
