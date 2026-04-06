using WalletService.Domain.Interfaces.Projections;

namespace WalletService.Domain.Interfaces;

public interface IWalletRepository
{
    Task CreateAsync(Wallet wallet);
    Task UpdateAsync(Wallet wallet, CancellationToken cancellationToken = default);
    Task<Wallet?> GetByIdAsync(WalletId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByDocumentNumberAsync(string documentNumber, CancellationToken cancellationToken = default);

    Task<WalletLimitProjection?> GetLimitByIdAsync(WalletId id, CancellationToken cancellationToken = default);
    Task<WalletInformationProjection?> GetInformationByIdAsync(WalletId id, CancellationToken cancellationToken = default);
}