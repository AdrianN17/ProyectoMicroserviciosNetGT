namespace TransactionService.Application.Wallets.Dtos;

public record WalletDto(
    Guid WalletId,
    string Name, 
    string LastName, 
    string DocumentType, 
    string DocumentNumber, 
    string Email, 
    string Phone, 
    string Status,
    string Currency, 
    decimal DailyLimit,
    decimal balanceAmount,
    Guid DailyLimitId
);