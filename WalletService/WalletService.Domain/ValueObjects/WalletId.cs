namespace WalletService.Domain.ValueObjects;

public readonly record struct WalletId(Guid Value)
{
    public static WalletId NewId() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}