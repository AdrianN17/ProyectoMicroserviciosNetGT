using ErrorOr;
using MediatR;
using TransactionService.Application.Transactions.Dtos;

namespace TransactionService.Application.Recharge.Queries.GetAllByWalletId;

public sealed record GetAllByWalletIdRechargeQuery(Guid WalletId) : IRequest<ErrorOr<IReadOnlyList<RechargeDto>>>;

