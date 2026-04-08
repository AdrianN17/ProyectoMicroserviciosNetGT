﻿using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using TransactionService.Application.Abstractions.Messaging;
using TransactionService.Application.Abstractions.Messaging.Sender;
using TransactionService.Application.Abstractions.Services;
using TransactionService.Application.Commmon.Interfaces;
using TransactionService.Application.Mapper;

namespace TransactionService.Application.Transactions.Commands.DeleteTransaction;

public sealed class DeleteTransactionCommandHandler : IRequestHandler<DeleteTransactionCommand, ErrorOr<Guid>>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteTransactionCommandHandler> _logger;
    private readonly IWalletReadService _walletReadService;
    private readonly IExcnangeReadService _exchangeReadService;
    private readonly IProducer _producer;

    public DeleteTransactionCommandHandler(ITransactionRepository transactionRepository, IWalletReadService walletReadService, IExcnangeReadService exchangeReadService,
        IUnitOfWork unitOfWork, ILogger<DeleteTransactionCommandHandler> logger, IProducer producer)
    {
        _transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _walletReadService = walletReadService ?? throw new ArgumentNullException(nameof(walletReadService));
        _exchangeReadService = exchangeReadService ?? throw new ArgumentNullException(nameof(exchangeReadService));
        _producer = producer ?? throw new ArgumentNullException(nameof(producer));
    }

    public async Task<ErrorOr<Guid>> Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Delete transaction with id {TransactionId}", request.TransactionId);

        var transactionId = new TransactionId(request.TransactionId);
        var transaction = await _transactionRepository.GetByIdAsync(transactionId);
        if(transaction is null) throw new InvalidOperationException("La Transaccion no existo o esta inactivo");
        
        var wallet = await _walletReadService.GetByIdAsync(transaction.FromWalletId.Value, cancellationToken);
        if(wallet is null) throw new InvalidOperationException("La Wallet no existo o esta inactivo");
        
        if (!EnumParsing.TryParseEnum<CurrencyType>(wallet.Currency, out var walletCurrency))
            return Error.Validation(code: "CurrencyType.Invalid", description: $"CurrencyType of Wallet '{wallet.Currency}' no es válido.");
        
        transaction.SoftDelete();

        await _transactionRepository.UpdateAsync(transaction);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var operations = transaction.ToOperation(walletCurrency);
        foreach (var operation in operations)
            await _producer.PublishAsync(operation.ToSendOperation(), cancellationToken);

        return transactionId.Value;
    }
}