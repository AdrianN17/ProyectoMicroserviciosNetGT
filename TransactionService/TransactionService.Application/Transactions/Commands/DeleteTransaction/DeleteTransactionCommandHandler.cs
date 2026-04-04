using ErrorOr;
using Microsoft.Extensions.Logging;
using TransactionService.Application.Commmon.Interfaces;
using TransactionService.Domain.Interfaces;
using TransactionService.Domain.ValueObjects;

namespace TransactionService.Application.Transactions.Commands.DeleteTransaction;

public sealed class DeleteTransactionCommandHandler
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteTransactionCommandHandler> _logger;
    
    public DeleteTransactionCommandHandler(ITransactionRepository transactionRepository, IUnitOfWork unitOfWork, ILogger<DeleteTransactionCommandHandler> logger)
    {
        _transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<ErrorOr<Guid>> Handle(DeleteTransactionCommandd request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Delete transaction with id {TransactionId}", request.TransactionId);

        var transactionId = new TransactionId(request.TransactionId);
        var transaction = await _transactionRepository.GetByIdAsync(transactionId);
        transaction.SoftDelete();
        
        await _transactionRepository.UpdateAsync(transaction);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return transactionId.Value;
    }
}