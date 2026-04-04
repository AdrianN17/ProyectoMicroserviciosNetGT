using MediatR;
using ErrorOr;

namespace TransactionService.Application.Transactions.Commands.CreateTransaction;

public sealed record CreateTransactionCommand(
    Guid FromWalletId,
    Guid ToWalletId,
    decimal Amount,
    string Currency,
    string SourceType
) : IRequest<ErrorOr<Guid>>;