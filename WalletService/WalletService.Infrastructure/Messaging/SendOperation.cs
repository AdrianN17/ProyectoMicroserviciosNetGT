using MassTransit;

namespace WalletService.Infrastructure.Messaging;

[MessageUrn("TransactionService.Application.Abstractions.Messaging.Sender:SendOperation")]
public sealed record SendOperation(
    string Type,
    string WalletId,
    string Amount,
    string Currency
);