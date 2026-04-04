namespace TransactionService.Domain.ValueObjects;

public readonly record struct WalletId(Guid Value)
{
    public override string ToString() => Value.ToString();
}