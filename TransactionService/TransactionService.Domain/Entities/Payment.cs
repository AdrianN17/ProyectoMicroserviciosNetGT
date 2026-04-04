using TransactionService.Domain.ValueObjects;
using WalletService.Domain.Common;
using WalletService.Domain.ValueObjects;

namespace TransactionService.Domain.Entities;

public class Transaction : AggregateRoot<WalletId>
{
    public WalletId FromWalletId { get; private set; }
    public WalletId ToWalletId { get; private set; }
    public Amount Amount { get; private set; }
}