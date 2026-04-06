using TransactionService.Application.Commmon.Interfaces;
using TransactionService.Infrastructure.Persistence.Contexts;

namespace TransactionService.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Cosmos DB no soporta transacciones distribuidas entre containers.
    /// Las operaciones atómicas se manejan a nivel de un solo item (Transactional Batch dentro del mismo partition key).
    /// Esta implementación mantiene la interfaz IUnitOfWork por compatibilidad con la capa Application.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => _context.SaveChangesAsync(cancellationToken);

        public void Dispose() => _context.Dispose();

        public ValueTask DisposeAsync() => _context.DisposeAsync();
    }
}
