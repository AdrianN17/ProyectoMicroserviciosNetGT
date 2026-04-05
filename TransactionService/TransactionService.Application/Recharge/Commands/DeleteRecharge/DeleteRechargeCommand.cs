using MediatR;
using ErrorOr;

namespace TransactionService.Application.Transactions.Commands.DeleteRecharge;

public sealed record DeleteRechargeCommand(
    Guid RechargeId
) : IRequest<ErrorOr<Guid>>;

