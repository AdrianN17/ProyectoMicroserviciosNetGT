﻿namespace WalletService.Domain.ValueObjects;

public readonly record struct WalletLimitId(Guid Value)
{
    public static WalletLimitId NewId() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}