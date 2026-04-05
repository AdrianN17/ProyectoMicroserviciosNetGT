using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using TransactionService.Application.Commmon.Interfaces;
using TransactionService.Domain.Interfaces;
using TransactionService.Domain.ValueObjects;

namespace TransactionService.Application.Transactions.Commands.DeleteRecharge;

public sealed class DeleteRechargeCommandHandler : IRequestHandler<DeleteRechargeCommand, ErrorOr<Guid>>
{
    private readonly IRechargeRepository _rechargeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteRechargeCommandHandler> _logger;

    public DeleteRechargeCommandHandler(IRechargeRepository rechargeRepository, IUnitOfWork unitOfWork,
        ILogger<DeleteRechargeCommandHandler> logger)
    {
        _rechargeRepository = rechargeRepository ?? throw new ArgumentNullException(nameof(rechargeRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ErrorOr<Guid>> Handle(DeleteRechargeCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Delete recharge with id {RechargeId}", request.RechargeId);

        var rechargeId = new RechargeId(request.RechargeId);
        var recharge = await _rechargeRepository.GetByIdAsync(rechargeId, cancellationToken);
        recharge!.SoftDelete();

        await _rechargeRepository.UpdateAsync(recharge, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return rechargeId.Value;
    }
}

