namespace TransactionService.Domain.Interfaces;

public interface ITransactionRepository
{
    Task CreateAsync(Transaction transaction);
    Task UpdateAsync(Transaction transaction, CancellationToken cancellationToken = default);
    Task<Transaction?> GetByIdAsync(TransactionId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Transaction>> GetAllByFromWalletIdPeerDayAsync(WalletId fromWalletId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Transaction>> GetAllByFromWalletId(WalletId fromWalletId, CancellationToken cancellationToken = default);
}