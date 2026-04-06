using ErrorOr;
using MediatR;
using TransactionService.Application.Transactions.Dtos;

namespace TransactionService.Application.Recharge.Queries;


public sealed record GetByIdRechargeQuery(Guid RechargeId) : IRequest<ErrorOr<RechargeDto>>;