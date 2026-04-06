namespace WalletService.Application.Wallets.Dtos;


public record WalletInformationDto(
    Guid WalletId,
    decimal DailyLimit,
    string Currency, 
    decimal balanceAmount
);