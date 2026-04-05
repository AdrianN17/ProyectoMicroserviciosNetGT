using TransactionService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace TransactionService.Infrastructure.Persistence.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {

        public TransactionRepository()
        {
            //_dbContext = dbContext;
        }

        public async Task CreateAsync(Transaction transaction)
        {
            //await _dbContext.Transactions.AddAsync(transaction);
        }
        
        public async Task<Transaction?> GetByIdAsync(TransactionId id, CancellationToken cancellationToken = default)
        {
            /*return await _dbContext.Transactions
                .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);*/
        }

        public async Task UpdateAsync(Transaction transaction, CancellationToken cancellationToken = default)
        {
            /*_dbContext.Transactions.Update(transaction);
            await Task.CompletedTask;*/
        }
    }
}
