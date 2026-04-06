using MediatR;
using ErrorOr;

namespace TransactionService.Application.Transactions.Commands.DeleteTransaction;

public sealed record DeleteTransactionCommand(
    Guid TransactionId
) : IRequest<ErrorOr<Guid>>;