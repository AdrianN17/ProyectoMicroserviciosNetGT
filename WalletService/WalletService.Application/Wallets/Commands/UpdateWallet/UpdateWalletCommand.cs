using ErrorOr;
using MediatR;

namespace WalletService.Application.Wallets.Commands.UpdateWallet;

public sealed record UpdateWalletCommand(
    Guid WalletId,
    string Name, 
    string LastName, 
    string DocumentType, 
    string DocumentNumber, 
    string Email, 
    string Phone, 
    string Currency, 
    decimal DailyLimit
) : IRequest<ErrorOr<Guid>>;