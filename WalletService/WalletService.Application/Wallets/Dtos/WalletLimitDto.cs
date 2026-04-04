namespace WalletService.Application.Wallets.Dtos;

public record WalletLimitDto(
    string Currency, 
    decimal DailyLimit
);