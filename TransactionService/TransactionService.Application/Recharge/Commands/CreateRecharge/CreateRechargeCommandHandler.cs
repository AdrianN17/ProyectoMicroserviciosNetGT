using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using TransactionService.Application.Commmon.Interfaces;
using TransactionService.Application.Transactions.Commands.CreateRecharge;
using DomainRecharge = TransactionService.Domain.Entities.Recharge;

namespace TransactionService.Application.Recharge.Commands.CreateRecharge;

public sealed class CreateRechargeCommandHandler : IRequestHandler<CreateRechargeCommand, ErrorOr<Guid>>
{
    private readonly IRechargeRepository _rechargeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateRechargeCommandHandler> _logger;

    public CreateRechargeCommandHandler(IRechargeRepository rechargeRepository, IUnitOfWork unitOfWork,
        ILogger<CreateRechargeCommandHandler> logger)
    {
        _rechargeRepository = rechargeRepository ?? throw new ArgumentNullException(nameof(rechargeRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ErrorOr<Guid>> Handle(CreateRechargeCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating recharge for wallet {WalletId}", request.WalletId);

        if (!EnumParsing.TryParseEnum<CurrencyType>(request.Currency, out var currency))
            return Error.Validation(code: "CurrencyType.Invalid", description: $"CurrencyType '{request.Currency}' no es válido.");

        if (!EnumParsing.TryParseEnum<MethodType>(request.MethodType, out var methodType))
            return Error.Validation(code: "MethodType.Invalid", description: $"MethodType '{request.MethodType}' no es válido.");

        var recharge = DomainRecharge.Create(
            walletId: request.WalletId,
            amount: request.Amount,
            currency: currency,
            methodType: methodType,
            rechargeStatus: RechargeStatus.PROCESANDO
        );

        await _rechargeRepository.CreateAsync(recharge);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return recharge.Id.Value;
    }
}

