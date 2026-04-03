namespace WalletService.Application.Wallets.Commands.UpdateWallet;

using WalletService.Application.Commmon.Interfaces;
using WalletService.Application.Common.Helpers;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;

public sealed class UpdateWalletCommandHandler : IRequestHandler<UpdateCustomerCommand, ErrorOr<Guid>>
{
    private readonly IWalletRepository _walletRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateWalletCommandHandler> _logger;
    
    public UpdateWalletCommandHandler(IWalletRepository walletRepository, IUnitOfWork unitOfWork, ILogger<UpdateWalletCommandHandler> logger)
    {
        _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<ErrorOr<Guid>> Handle(UpdateWalletCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating wallet with document number {DocumentNumber}", command.DocumentNumber);

        if (!EnumParsing.TryParseEnum<DocumentType>(command.DocumentType, out var documentType))
            return Error.Validation(code: "DocumentType.Invalid", description: $"DocumentType '{command.DocumentType}' no es válido.");
        
        if (!EnumParsing.TryParseEnum<CurrencyType>(command.Currency, out var currency))
            return Error.Validation(code: "CurrencyType.Invalid", description: $"CurrencyType '{command.Currency}' no es válido.");
        
        var wallet = Wallet.Create(
            request.Name, 
            request.LastName, 
            documentType, 
            request.DocumentNumber, 
            request.Email, 
            request.Phone, 
            currency, 
            request.DailyLimit,
            request.WalletLimitId
        );
        
        _walletRepository.UpdateAsync(wallet);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return wallet.Id.Value;
    }
}