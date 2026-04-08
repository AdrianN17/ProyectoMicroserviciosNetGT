namespace TransactionService.Application.Abstractions.Messaging.Sender;

public sealed record SendOperation(
    string Type,
    Guid WalletId,
    decimal Amount,
    string Currency
);