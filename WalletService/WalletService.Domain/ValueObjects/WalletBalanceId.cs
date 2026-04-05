namespace WalletService.Domain.ValueObjects;

public readonly record struct WalletBalanceId(Guid Value)
{
public static WalletBalanceId NewId() => new(Guid.NewGuid());
public override string ToString() => Value.ToString();
}