using WalletService.Application.Wallets.Dtos;
using ErrorOr;
using MediatR;

namespace WalletService.Application.Wallets.Queries.GetByIdWallet;
public sealed record GetByIdWalletQuery(Guid WalletId) : IRequest<ErrorOr<WalletDto>>;
