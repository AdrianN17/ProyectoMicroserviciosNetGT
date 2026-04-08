using ErrorOr;
using MediatR;
using TransactionService.Application.Transactions.Dtos;

namespace TransactionService.Application.Recharge.Queries.GetAllByWalletId;

public sealed class GetAllByWalletIdRechargeQueryHandler
    : IRequestHandler<GetAllByWalletIdRechargeQuery, ErrorOr<IReadOnlyList<RechargeDto>>>
{
    private readonly IRechargeRepository _rechargeRepository;

    public GetAllByWalletIdRechargeQueryHandler(IRechargeRepository rechargeRepository)
    {
        _rechargeRepository = rechargeRepository;
    }

    public async Task<ErrorOr<IReadOnlyList<RechargeDto>>> Handle(
        GetAllByWalletIdRechargeQuery request, CancellationToken cancellationToken)
    {
        var recharges = await _rechargeRepository.GetAllByFromWalletId(
            new WalletId(request.WalletId), cancellationToken);

        if (!recharges.Any())
            return Error.NotFound("Recharge.NotFound",
                $"No se encontraron recargas para la wallet {request.WalletId}.");

        return recharges.Select(r => new RechargeDto(
            r.Id.Value,
            r.WalletId.Value,
            r.Amount.Value,
            r.Amount.Currency.ToString(),
            r.MethodType.ToString(),
            r.RechargeStatus.ToString()
        )).ToList();
    }
}

