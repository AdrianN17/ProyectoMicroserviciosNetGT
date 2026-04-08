﻿namespace WalletService.Domain.Interfaces.Projections;

public record WalletInformationProjection(
    Guid WalletId,
    decimal DailyLimit,
    string Currency,
    decimal BalanceAmount
);