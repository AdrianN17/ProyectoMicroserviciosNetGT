using ErrorOr;
using MediatR;
using WalletService.Application.Wallets.Dtos;

namespace WalletService.Application.Wallets.Queries.GetInformationByWallet;

public sealed class GetInformationByWalletQueryHandler : IRequestHandler<GetInformationByWalletQuery, ErrorOr<WalletInformationDto>>
{
    private readonly IWalletRepository _walletRepository;

    public GetInformationByWalletQueryHandler(IWalletRepository walletRepository)
    {
        _walletRepository = walletRepository;
    }

    public async Task<ErrorOr<WalletInformationDto>> Handle(GetInformationByWalletQuery request, CancellationToken cancellationToken)
    {
        var walletInformation = await _walletRepository.GetInformationByIdAsync(new WalletId(request.WalletId), cancellationToken);

        if (walletInformation is null)
        {
            return Error.NotFound("walletInformation.NotFound", $"Wallet with id {request.WalletId} not found.");
        }

        return new WalletInformationDto(
            walletInformation.WalletId,
            walletInformation.DailyLimit,
            walletInformation.Currency,
            walletInformation.balanceAmount
        );
    }
}

