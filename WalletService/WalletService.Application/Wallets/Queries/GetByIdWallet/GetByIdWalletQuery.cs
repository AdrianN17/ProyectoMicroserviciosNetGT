using ErrorOr;
using MediatR;
using WalletService.Application.Wallets.Dtos;

namespace WalletService.Application.Wallets.Queries.GetByIdWallet;
public sealed record GetByIdWalletQuery(Guid WalletId) : IRequest<ErrorOr<WalletDto>>;
