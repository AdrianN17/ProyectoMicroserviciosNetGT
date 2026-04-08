using ErrorOr;
using MediatR;
using TransactionService.Application.Transactions.Dtos;

namespace TransactionService.Application.Transactions.Queries.GetAllByFromWalletId;

public sealed record GetAllByFromWalletIdTransactionQuery(Guid FromWalletId) : IRequest<ErrorOr<IReadOnlyList<TransactionDto>>>;

