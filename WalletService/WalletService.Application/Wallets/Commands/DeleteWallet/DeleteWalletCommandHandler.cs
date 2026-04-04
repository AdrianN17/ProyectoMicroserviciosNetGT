namespace WalletService.Application.Wallets.Commands.DeleteWallet;

using WalletService.Application.Commmon.Interfaces;
using WalletService.Application.Common.Helpers;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using WalletService.Domain.Enums;

public sealed class DeleteWalletCommandHandler : IRequestHandler<DeleteWalletCommand, ErrorOr<Guid>>
{
    private readonly IWalletRepository _walletRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteWalletCommandHandler> _logger;
    
    public DeleteWalletCommandHandler(IWalletRepository walletRepository, IUnitOfWork unitOfWork, ILogger<DeleteWalletCommandHandler> logger)
    {
        _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<ErrorOr<Guid>> Handle(DeleteWalletCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Delete wallet with document number {WalletId}", request.WalletId);

        var walletId = new WalletId(request.WalletId);
        var wallet = await _walletRepository.GetByIdAsync(walletId);
        wallet.Suspend();
        wallet.SoftDelete();
        wallet.WalletLimit.softDelete();
        
        
        await _walletRepository.UpdateAsync(wallet);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return walletId.Value;
    }
}