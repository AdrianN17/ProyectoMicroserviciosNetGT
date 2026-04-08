using TransactionService.Application.Abstractions.Messaging.Sender;

namespace TransactionService.Application.Mapper;

public static class OperationMapper
{
    public static SendOperation ToSendOperation(this Operation operation) =>
        new SendOperation(
            Type: operation.Type.ToString(),
            WalletId: operation.WalletId.Value,
            Amount: operation.Amount,
            Currency: operation.Currency.ToString()
        );
}