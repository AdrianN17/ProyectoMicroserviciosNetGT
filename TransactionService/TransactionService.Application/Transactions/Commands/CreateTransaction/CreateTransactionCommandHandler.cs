﻿﻿using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using TransactionService.Application.Abstractions.Messaging;
using TransactionService.Application.Abstractions.Services;
using TransactionService.Application.Commmon.Interfaces;
using TransactionService.Application.Mapper;

namespace TransactionService.Application.Transactions.Commands.CreateTransaction;

public sealed class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, ErrorOr<Guid>>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateTransactionCommandHandler> _logger;
    private readonly IWalletReadService _walletReadService;
    private readonly IExcnangeReadService _exchangeReadService;
    private readonly IProducer _producer;
    
    public CreateTransactionCommandHandler(ITransactionRepository transactionRepository, IWalletReadService walletReadService, IExcnangeReadService exchangeReadService,
        IUnitOfWork unitOfWork, ILogger<CreateTransactionCommandHandler> logger, IProducer producer)
    {
        _transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _walletReadService = walletReadService ?? throw new ArgumentNullException(nameof(walletReadService));
        _exchangeReadService = exchangeReadService ?? throw new ArgumentNullException(nameof(exchangeReadService));
        _producer = producer ?? throw new ArgumentNullException(nameof(producer));
    }
    
    public async Task<ErrorOr<Guid>> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating transaction from {FromWalletId} to {ToWalletId}", request.ToWalletId, request.ToWalletId);
        
        var walletFrom = await _walletReadService.GetByIdAsync(request.FromWalletId, cancellationToken);
        if(walletFrom is null) throw new InvalidOperationException("La Wallet origen no existo o esta inactivo");
        
        var walletTo = await _walletReadService.GetByIdAsync(request.ToWalletId, cancellationToken);
        if(walletTo is null) throw new InvalidOperationException("La Wallet destino no existo o esta inactivo");
        
        if (!EnumParsing.TryParseEnum<CurrencyType>(request.Currency, out var currency))
            return Error.Validation(code: "CurrencyType.Invalid", description: $"CurrencyType '{request.Currency}' no es válido.");
        
        if (!EnumParsing.TryParseEnum<SourceType>(request.SourceType, out var sourceType))
            return Error.Validation(code: "SourceType.Invalid", description: $"SourceType '{request.SourceType}' no es válido.");
        
        if (!EnumParsing.TryParseEnum<CurrencyType>(walletFrom.Currency, out var walletFromCurrency))
            return Error.Validation(code: "CurrencyType.Invalid", description: $"CurrencyType of walletFrom '{walletFrom.Currency}' no es válido.");
        if (!EnumParsing.TryParseEnum<CurrencyType>(walletTo.Currency, out var walletToCurrency))
            return Error.Validation(code: "CurrencyType.Invalid", description: $"CurrencyType of walletTo '{walletTo.Currency}' no es válido.");
        
        var exchange = await _exchangeReadService.GetByCurrencyTypeAsync(currency, cancellationToken);
        if(exchange is null) throw new InvalidOperationException("El tipo de cambio no existo o esta inactivo");

        var transactions =
            await _transactionRepository.GetAllByFromWalletIdPeerDayAsync(new WalletId(request.FromWalletId));
        
        
        var transaction = Transaction.Create(
            fromWalletId: request.FromWalletId,
            toWalletId: request.ToWalletId,
            amount: request.Amount,
            currency: currency,
            sourceType: sourceType,
            exchangeRate: exchange.value
        );
        
        var amountTransactions = transactions?.Sum(t => t.TotalCalculated(currency)) ?? 0m;
        
        transaction.ValidateIfTransactionHaveLimit(walletFrom.DailyLimit, amountTransactions, walletFromCurrency);
        transaction.TotalCalculatedToWalletFrom(walletFrom.balanceAmount, walletFromCurrency);
        
        await _transactionRepository.CreateAsync(transaction);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        var operations = transaction.ToOperation(walletFromCurrency);
        foreach (var operation in operations)
            await _producer.PublishAsync(operation.ToSendOperation(), cancellationToken);

        return transaction.Id.Value;
    }
}