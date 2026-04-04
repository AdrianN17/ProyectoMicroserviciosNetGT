﻿using WalletService.Domain.Interfaces;
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

        public async Task CreateAsync(Wallet wallet)
        {
            await _dbContext.Wallets.AddAsync(wallet);
        }

        public async Task<bool> ExistsByDocumentNumberAsync(string documentNumber, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(documentNumber)) return false;

            documentNumber = documentNumber.Trim();

            return await _dbContext.Wallets.AsNoTracking()
                .Where(e => e.Document.Number == documentNumber)
                .AnyAsync(cancellationToken);
        }

        public async Task<Wallet?> GetByIdAsync(WalletId id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Wallets
                .Include(a => a.WalletLimit)
                .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
        }

        public async Task UpdateAsync(Wallet wallet, CancellationToken cancellationToken = default)
        {
            _dbContext.Wallets.Update(wallet);
            await Task.CompletedTask;
        }
    }
}
