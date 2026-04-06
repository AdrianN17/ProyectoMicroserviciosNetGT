using Microsoft.EntityFrameworkCore;
using TransactionService.Domain.Interfaces;
using TransactionService.Infrastructure.Persistence.Contexts;

namespace TransactionService.Infrastructure.Persistence.Repositories
{
    public class TransactionRepository(ApplicationDbContext context) : ITransactionRepository
    {
        public async Task CreateAsync(Transaction transaction)
        {
            await context.Transactions.AddAsync(transaction);
            await context.SaveChangesAsync();
        }

        public async Task<Transaction?> GetByIdAsync(TransactionId id, CancellationToken cancellationToken = default)
        {
            return await context.Transactions
                .Where(t => t.Id == id && !t.IsDeleted)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task UpdateAsync(Transaction transaction, CancellationToken cancellationToken = default)
        {
            context.Transactions.Update(transaction);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
