using System.Globalization;
using TransactionService.Application.Abstractions.Messaging.Sender;

namespace TransactionService.Application.Mapper;

public static class OperationMapper
{
    public static SendOperation ToSendOperation(this Operation operation) =>
        new SendOperation(
            Type: operation.Type.ToString(),
            WalletId: operation.WalletId.Value.ToString(),
            Amount: operation.Amount.ToString(CultureInfo.InvariantCulture),
            Currency: operation.Currency.ToString()
        );
}