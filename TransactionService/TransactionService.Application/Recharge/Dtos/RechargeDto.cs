namespace TransactionService.Application.Transactions.Dtos;

public record RechargeDto(
    Guid Id,
    Guid WalletId,
    decimal Amount,
    string Currency,
    string MethodType,
    string RechargeStatus
);