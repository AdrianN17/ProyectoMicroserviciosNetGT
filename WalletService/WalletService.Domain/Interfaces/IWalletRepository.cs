namespace WalletService.Domain.Interfaces;

public interface IWalletRepository
{
    void CreateAsync(Wallet wallet);
    Task UpdateAsync(Wallet wallet, CancellationToken cancellationToken = default);
    Task DeleteAsync(WalletId id, CancellationToken cancellationToken = default);
    Task<Wallet?> GetByIdAsync(WalletId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByDocumentNumberAsync(string documentNumber, CancellationToken cancellationToken = default);

}