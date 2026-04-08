using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using TransactionService.Application.Abstractions.Messaging;
using TransactionService.Application.Abstractions.Services;
using TransactionService.Application.Commmon.Interfaces;
using TransactionService.Domain.Interfaces;
using TransactionService.Domain.ValueObjects;

namespace TransactionService.Application.Transactions.Commands.DeleteRecharge;

public sealed class DeleteRechargeCommandHandler : IRequestHandler<DeleteRechargeCommand, ErrorOr<Guid>>
{
    private readonly IRechargeRepository _rechargeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteRechargeCommandHandler> _logger;
    private readonly IWalletReadService _walletReadService;
    private readonly IExcnangeReadService _exchangeReadService;
    private readonly IProducer _producer;

    public DeleteRechargeCommandHandler(IRechargeRepository rechargeRepository, IWalletReadService walletReadService, IExcnangeReadService exchangeReadService,
        IUnitOfWork unitOfWork, ILogger<DeleteRechargeCommandHandler> logger, IProducer producer)
    {
        _rechargeRepository = rechargeRepository ?? throw new ArgumentNullException(nameof(rechargeRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _walletReadService = walletReadService ?? throw new ArgumentNullException(nameof(walletReadService));
        _exchangeReadService = exchangeReadService ?? throw new ArgumentNullException(nameof(exchangeReadService));
        _producer = producer ?? throw new ArgumentNullException(nameof(producer));
    }

    public async Task<ErrorOr<Guid>> Handle(DeleteRechargeCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Delete recharge with id {RechargeId}", request.RechargeId);
        
        
        var rechargeId = new RechargeId(request.RechargeId);
        var recharge = await _rechargeRepository.GetByIdAsync(rechargeId, cancellationToken);
        if(recharge is null) throw new InvalidOperationException("La Recarga no existo o esta inactivo");
        
        var wallet = await _walletReadService.GetByIdAsync(recharge.WalletId.Value, cancellationToken);
        if(wallet is null) throw new InvalidOperationException("La Wallet no existo o esta inactivo");
        
        if (!EnumParsing.TryParseEnum<CurrencyType>(wallet.Currency, out var walletCurrency))
            return Error.Validation(code: "CurrencyType.Invalid", description: $"CurrencyType of Wallet '{wallet.Currency}' no es válido.");

        
        recharge!.SoftDelete();

        await _rechargeRepository.UpdateAsync(recharge, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        var operation = recharge.ToOperation(walletCurrency);
        await _producer.PublishAsync(operation, cancellationToken);

        return rechargeId.Value;
    }
}

