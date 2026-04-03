
using WalletService.Domain.Interfaces;
using WalletService.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace WalletService.Infrastructure.Persistence.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public WalletRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void CreateAsync(Wallet wallet)
        {
            _dbContext.Wallets.Add(wallet);
        }

        public Task DeleteAsync(WalletId id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ExistsByDocumentNumberAsync(string documentNumber, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(documentNumber)) return false;

            documentNumber = documentNumber.Trim();

            return await _dbContext.Wallets.AsNoTracking()
                .Where(e => e.Document.Number == documentNumber)
                .AnyAsync();
        }

        public async Task<Customer?> GetByIdAsync(WalletId id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Wallets
                .Include(a => a.Address)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public Task UpdateAsync(Wallet wallet, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
