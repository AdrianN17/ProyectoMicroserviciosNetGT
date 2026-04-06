using ErrorOr;
using MediatR;
using TransactionService.Application.Transactions.Dtos;

namespace TransactionService.Application.Transactions.Queries;

public sealed record GetByIdTransactionQuery(Guid TransactionId) : IRequest<ErrorOr<TransactionDto>>;

