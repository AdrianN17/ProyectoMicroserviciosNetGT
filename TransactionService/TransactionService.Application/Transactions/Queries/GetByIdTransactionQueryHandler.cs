using ErrorOr;
using MediatR;
using TransactionService.Application.Transactions.Dtos;

namespace TransactionService.Application.Transactions.Queries;

public sealed class GetByIdTransactionQueryHandler : IRequestHandler<GetByIdTransactionQuery, ErrorOr<TransactionDto>>
{
    private readonly ITransactionRepository _transactionRepository;

    public GetByIdTransactionQueryHandler(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<ErrorOr<TransactionDto>> Handle(GetByIdTransactionQuery request, CancellationToken cancellationToken)
    {
        var transaction = await _transactionRepository.GetByIdAsync(new TransactionId(request.TransactionId), cancellationToken);

        if (transaction is null)
            return Error.NotFound("Transaction.NotFound", $"Transaction with id {request.TransactionId} not found.");

        return new TransactionDto(
            transaction.Id.Value,
            transaction.FromWalletId.Value,
            transaction.ToWalletId.Value,
            transaction.Amount.Value,
            transaction.Amount.Currency.ToString(),
            transaction.SourceType.ToString(),
            transaction.TransactionStatus.ToString()
        );
    }
}

