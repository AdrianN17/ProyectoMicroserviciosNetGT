namespace TransactionService.Domain.ValueObjects;

public sealed class Operation
{
    public TypeOperation Type { get; init; }
    public WalletId WalletId { get; init; }
    public decimal Amount { get; init; }
    public CurrencyType Currency { get; init; }
}