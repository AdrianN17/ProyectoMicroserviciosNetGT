using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using TransactionService.Application.Commmon.Interfaces;
using TransactionService.Application.Common.Helpers;
using TransactionService.Domain.Entities;
using TransactionService.Domain.Enums;
using TransactionService.Domain.Interfaces;

namespace TransactionService.Application.Transactions.Commands.CreateTransaction;

public sealed class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, ErrorOr<Guid>>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateTransactionCommandHandler> _logger;
    
    public CreateTransactionCommandHandler(ITransactionRepository transactionRepository, IUnitOfWork unitOfWork, ILogger<CreateTransactionCommandHandler> logger)
    {
        _transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<ErrorOr<Guid>> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating transaction from {FromWalletId} to {ToWalletId}", request.FromWalletId, request.ToWalletId);
        
        if (!EnumParsing.TryParseEnum<CurrencyType>(request.Currency, out var currency))
            return Error.Validation(code: "CurrencyType.Invalid", description: $"CurrencyType '{request.Currency}' no es válido.");
        
        if (!EnumParsing.TryParseEnum<SourceType>(request.SourceType, out var sourceType))
            return Error.Validation(code: "SourceType.Invalid", description: $"SourceType '{request.SourceType}' no es válido.");
        
        var transaction = Transaction.Create(
            fromWalletId: request.FromWalletId,
            toWalletId: request.ToWalletId,
            amount: request.Amount,
            currency: currency,
            sourceType: sourceType
        );
        
        await _transactionRepository.CreateAsync(transaction);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return transaction.Id.Value;
    }
}