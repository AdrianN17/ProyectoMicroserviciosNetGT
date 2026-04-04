using ErrorOr;
using MediatR;
using WalletService.Application.Wallets.Dtos;

namespace WalletService.Application.Wallets.Queries.GetLimitByIdWalletLimit;

public sealed record GetByIdWalletLimitQuery(Guid WalletId) : IRequest<ErrorOr<WalletLimitDto>>;