namespace WalletService.Domain.Common;

public readonly record struct WalletLimitId(Guid value)
{
    public static WalletLimitId NewId() => new(Guid.NewGuid());
    public override string ToString() => value.ToString();
}