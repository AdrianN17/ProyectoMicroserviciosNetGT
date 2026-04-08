using ErrorOr;
using MediatR;
using TransactionService.Application.Transactions.Dtos;

namespace TransactionService.Application.Transactions.Queries.GetAllByFromWalletId;

public sealed class GetAllByFromWalletIdTransactionQueryHandler
    : IRequestHandler<GetAllByFromWalletIdTransactionQuery, ErrorOr<IReadOnlyList<TransactionDto>>>
{
    private readonly ITransactionRepository _transactionRepository;

    public GetAllByFromWalletIdTransactionQueryHandler(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<ErrorOr<IReadOnlyList<TransactionDto>>> Handle(
        GetAllByFromWalletIdTransactionQuery request, CancellationToken cancellationToken)
    {
        var transactions = await _transactionRepository.GetAllByFromWalletId(
            new WalletId(request.FromWalletId), cancellationToken);

        if (!transactions.Any())
            return Error.NotFound("Transaction.NotFound",
                $"No se encontraron transacciones para la wallet {request.FromWalletId}.");

        return transactions.Select(t => new TransactionDto(
            t.Id.Value,
            t.FromWalletId.Value,
            t.ToWalletId.Value,
            t.Amount.Value,
            t.Amount.Currency.ToString(),
            t.SourceType.ToString(),
            t.TransactionStatus.ToString()
        )).ToList();
    }
}

