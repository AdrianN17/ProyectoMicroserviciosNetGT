using MediatR;
using ErrorOr;

namespace WalletService.Application.Wallets.Commands.DeleteWallet;

public sealed record DeleteWalletCommand(
    Guid WalletId
) : IRequest<ErrorOr<Guid>>;
