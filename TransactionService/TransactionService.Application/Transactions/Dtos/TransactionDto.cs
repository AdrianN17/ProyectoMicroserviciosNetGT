namespace TransactionService.Application.Transactions.Dtos;

public record TransactionDto(
    Guid Id,
    Guid FromWalletId,
    Guid ToWalletId,
    decimal Amount,
    string Currency,
    string SourceType,
    string TransactionStatus
);