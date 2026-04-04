using ErrorOr;
using MediatR;
using WalletService.Application.Wallets.Dtos;

namespace WalletService.Application.Wallets.Queries.GetByIdWallet;

public sealed class GetByIdWalletQueryHandler : IRequestHandler<GetByIdWalletQuery, ErrorOr<WalletDto>>
{
    private readonly IWalletRepository _walletRepository;

    public GetByIdWalletQueryHandler(IWalletRepository walletRepository)
    {
        this._walletRepository = walletRepository;
    }
    
    public async Task<ErrorOr<WalletDto>> Handle(GetByIdWalletQuery request, CancellationToken cancellationToken)
    {
        var wallet = await _walletRepository.GetByIdAsync(new WalletId(request.WalletId));
        if (wallet == null) {
            return Error.NotFound("Wallet.NotFound", $"Wallet with id {request.WalletId} not found.");
        }

        return new WalletDto(
            wallet!.Id.Value,
            wallet.Name,
            wallet.LastName,
            wallet.Document.Type.ToString(),
            wallet.Document.Number,
            wallet.Email.ToString(),
            wallet.Phone.ToString(),
            wallet.WalletStatus.ToString(),
            wallet.WalletLimit.Currency.ToString(),
            wallet.WalletLimit.DailyLimit,
            wallet.WalletLimit.Id.Value
        );
    }
}