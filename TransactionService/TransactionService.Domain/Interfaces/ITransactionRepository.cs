using TransactionService.Domain.Entities;
using TransactionService.Domain.ValueObjects;

namespace TransactionService.Domain.Interfaces;

public interface ITransactionRepository
{
    Task CreateAsync(Transaction transaction);
    Task UpdateAsync(Transaction transaction, CancellationToken cancellationToken = default);
    Task<Transaction?> GetByIdAsync(TransactionId id, CancellationToken cancellationToken = default);
    Task<bool> GetWalletValidation(WalletId from, WalletId to, CancellationToken cancellationToken = default);
}