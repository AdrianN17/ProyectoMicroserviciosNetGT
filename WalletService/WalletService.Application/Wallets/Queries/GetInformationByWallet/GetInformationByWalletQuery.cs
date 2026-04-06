using ErrorOr;
using MediatR;
using WalletService.Application.Wallets.Dtos;

namespace WalletService.Application.Wallets.Queries.GetInformationByWallet;

public sealed record GetInformationByWalletQuery(Guid WalletId) : IRequest<ErrorOr<WalletInformationDto>>;
