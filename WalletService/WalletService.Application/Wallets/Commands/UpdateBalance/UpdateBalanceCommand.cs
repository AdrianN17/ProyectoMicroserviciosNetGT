using ErrorOr;
using MediatR;

namespace WalletService.Application.Wallets.Commands.UpdateBalance;

public sealed record UpdateBalanceCommand(
    string Type,
    Guid WalletId,
    decimal Amount,
    string Currency
) : IRequest<ErrorOr<Guid>>;
