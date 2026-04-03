using WalletService.Application.Wallets.Dtos;
using WalletService.Domain.Enums;
using ErrorOr;
using MediatR;

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

        return new WalletDto
        {
            WalletId = wallet!.Id.Value,
            Name = wallet.Name,
            LastName = wallet.LastName,
            DocumentType = wallet.Document.Type.ToString(),
            DocumentNumber = wallet.Document.Number,
            Email = wallet.Email,
            Phone = wallet.Phone,
            Currency = wallet.WalletLimit.Currency.ToString(),
            DailyLimit = wallet.WalletLimit.DailyLimit,
            DailyLimitId = wallet.WalletLimit.Id.Value,
            WalletStatus = wallet.Status.ToString()
        };
    }
}