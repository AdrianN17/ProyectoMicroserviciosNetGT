namespace WalletService.Application.Wallets.Dtos;

public record WalletDto(
    Guid WalletId,
    string Name, 
    string LastName, 
    string DocumentType, 
    string DocumentNumber, 
    string Email, 
    string Phone, 
    string Currency, 
    decimal DailyLimit,
    Guid DailyLimitId
)