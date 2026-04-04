using MediatR;
using ErrorOr;

namespace TransactionService.Application.Transactions.Commands.DeleteTransaction;

public sealed record DeleteTransactionCommandd(
    Guid TransactionId
) : IRequest<ErrorOr<Guid>>;