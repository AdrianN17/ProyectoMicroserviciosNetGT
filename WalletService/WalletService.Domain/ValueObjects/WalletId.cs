namespace WalletService.Domain.Common;

public readonly record struct WalletId(Guid value)
{
    public static WalletId NewId() => new(Guid.NewGuid());
    public override string ToString() => value.ToString();
}