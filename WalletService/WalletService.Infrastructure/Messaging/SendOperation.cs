namespace WalletService.Infrastructure.Messaging;

public sealed record SendOperation(
    string Type,
    string WalletId,
    string Amount,
    string Currency
);