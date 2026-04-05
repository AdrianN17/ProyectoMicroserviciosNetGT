using TransactionService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace TransactionService.Infrastructure.Persistence.Repositories
{
    public class RechargeRepository : IRechargeRepository
    {

        public RechargeRepository()
        {
            //_dbContext = dbContext;
        }

        public async Task CreateAsync(Recharge recharge)
        {
            //await _dbContext.Transactions.AddAsync(transaction);
        }

        public async Task<Recharge?> GetByIdAsync(RechargeId id, CancellationToken cancellationToken = default)
        {
            /*return await _dbContext.Transactions
                .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);*/
        }

        public async Task UpdateAsync(Recharge recharge, CancellationToken cancellationToken = default)
        {
            /*_dbContext.Transactions.Update(transaction);
            await Task.CompletedTask;*/
        }
    }
}
