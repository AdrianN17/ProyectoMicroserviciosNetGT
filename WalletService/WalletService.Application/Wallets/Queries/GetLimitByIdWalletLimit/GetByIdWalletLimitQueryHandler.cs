using ErrorOr;
using MediatR;
using WalletService.Application.Wallets.Dtos;

namespace WalletService.Application.Wallets.Queries.GetLimitByIdWalletLimit;

public sealed class GetByIdWalletLimitQueryHandler : IRequestHandler<GetByIdWalletLimitQuery, ErrorOr<WalletLimitDto>>
{
    private readonly IWalletRepository _walletRepository;

    public GetByIdWalletLimitQueryHandler(IWalletRepository walletRepository)
    {
        this._walletRepository = walletRepository;
    }
    
    public async Task<ErrorOr<WalletLimitDto>> Handle(GetByIdWalletLimitQuery request, CancellationToken cancellationToken)
    {
        var walletLimit = await _walletRepository.GetLimitByIdAsync(new WalletId(request.WalletId));
        if (walletLimit == null) {
            return Error.NotFound("walletLimit.NotFound", $"walletLimit with id {request.WalletId} not found.");
        }

        return new WalletLimitDto(
            walletLimit.Currency,
            walletLimit.DailyLimit
        );
    }
}