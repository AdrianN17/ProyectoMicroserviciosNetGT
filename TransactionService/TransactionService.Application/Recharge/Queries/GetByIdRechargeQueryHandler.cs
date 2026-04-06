using ErrorOr;
using MediatR;
using TransactionService.Application.Transactions.Dtos;

namespace TransactionService.Application.Recharge.Queries;

public sealed class GetByIdRechargeQueryHandler : IRequestHandler<GetByIdRechargeQuery, ErrorOr<RechargeDto>>
{
    private readonly IRechargeRepository _rechargeRepository;

    public GetByIdRechargeQueryHandler(IRechargeRepository rechargeRepository)
    {
        this._rechargeRepository = rechargeRepository;
    }
    
    public async Task<ErrorOr<RechargeDto>> Handle(GetByIdRechargeQuery request, CancellationToken cancellationToken)
    {
        var recharge = await _rechargeRepository.GetByIdAsync(new RechargeId(request.RechargeId));
        if (recharge == null) {
            return Error.NotFound("Recharge.NotFound", $"Recharge with id {request.RechargeId} not found.");
        }

        return new RechargeDto(
            recharge!.Id.Value,
            recharge.WalletId.Value,
            recharge.Amount.Value,
            recharge.Amount.Currency.ToString(),
            recharge.MethodType.ToString(),
            recharge.RechargeStatus.ToString()
        );
    }
}