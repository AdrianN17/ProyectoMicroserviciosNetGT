using TransactionService.Application.Wallets.Dtos;

namespace TransactionService.Application.Abstractions.Services;

public interface IWalletReadService
{
    Task<WalletDto?> GetByIdAsync(Guid walletId, CancellationToken cancellationToken);
}