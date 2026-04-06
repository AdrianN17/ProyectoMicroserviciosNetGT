using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using TransactionService.Application.Abstractions.Services;
using TransactionService.Application.Commmon.Interfaces;

namespace TransactionService.Application.Transactions.Commands.CreateTransaction;

public sealed class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, ErrorOr<Guid>>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateTransactionCommandHandler> _logger;
    private readonly IWalletReadService _walletReadService;
    private readonly IExcnangeReadService _exchangeReadService;
    
    public CreateTransactionCommandHandler(ITransactionRepository transactionRepository, IWalletReadService walletReadService, IExcnangeReadService exchangeReadService,
        IUnitOfWork unitOfWork, ILogger<CreateTransactionCommandHandler> logger)
    {
        _transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _walletReadService = walletReadService ?? throw new ArgumentNullException(nameof(walletReadService));
        _exchangeReadService = exchangeReadService ?? throw new ArgumentNullException(nameof(exchangeReadService));
    }
    
    public async Task<ErrorOr<Guid>> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating transaction from {FromWalletId} to {ToWalletId}", request.FromWalletId, request.ToWalletId);
        
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
        
        
        var balanceFrom = walletFrom.balanceAmount;
        var rechargeAmountFrom = request.Amount;

        if (walletFromCurrency != currency)
        {
            var exchange = await _exchangeReadService.GetByCurrencyTypeAsync(currency, cancellationToken);
            if(exchange is null) throw new InvalidOperationException("El tipo de cambio no existo o esta inactivo");
            
            rechargeAmountFrom *= exchange.value;
        }
            
        if(balanceFrom < rechargeAmountFrom)
            return Error.Validation(code: "Amount.Invalid", description: $"El monto de transferencia no puede ser mayor al balance actual de la wallet.");
        
        

        /*var amountExchangeFrom = request.Amount;
        var amountExchangeTo = request.Amount;

        if (walletFromCurrency != currency)
        {
            var exchangeFrom = await _exchangeReadService.GetByCurrencyTypeAsync(walletFromCurrency, cancellationToken);
            if(exchangeFrom is null) throw new InvalidOperationException("El tipo de cambio de walletFrom no existo o esta inactivo");

            amountExchangeFrom *= exchangeFrom.value;
        }

        if (walletToCurrency != currency)
        {
            var exchangeTo = await _exchangeReadService.GetByCurrencyTypeAsync(walletToCurrency, cancellationToken);
            if(exchangeTo is null) throw new InvalidOperationException("El tipo de cambio de walletTo no existo o esta inactivo");

            amountExchangeFrom *= exchangeTo.value;
        }*/
        
        
        
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