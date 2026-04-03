namespace WalletService.Application.Wallets.Commands.CreateWallet;

public sealed record CreateWalletCommand(
    string Name, 
    string LastName, 
    string DocumentType, 
    string DocumentNumber, 
    string Email, 
    string Phone, 
    string Currency, 
    decimal DailyLimit
)