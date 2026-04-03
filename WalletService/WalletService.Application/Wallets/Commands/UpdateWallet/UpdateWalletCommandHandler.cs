namespace WalletService.Application.Wallets.Commands.UpdateWallet;

using WalletService.Application.Commmon.Interfaces;
using WalletService.Application.Common.Helpers;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using WalletService.Domain.Common;

public sealed class UpdateWalletCommandHandler : IRequestHandler<UpdateWalletCommand, ErrorOr<Guid>>
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
        _logger.LogInformation("Updating wallet with document number {DocumentNumber}", request.DocumentNumber);

        if (!EnumParsing.TryParseEnum<DocumentType>(request.DocumentType, out var documentType))
            return Error.Validation(code: "DocumentType.Invalid", description: $"DocumentType '{request.DocumentType}' no es válido.");
        
        if (!EnumParsing.TryParseEnum<CurrencyType>(request.Currency, out var currency))
            return Error.Validation(code: "CurrencyType.Invalid", description: $"CurrencyType '{request.Currency}' no es válido.");
        
        // Buscar la wallet existente y aplicar cambios usando las operaciones del dominio.
        var wallet = await _walletRepository.GetByIdAsync(new WalletId(request.WalletId));
        if (wallet == null)
        {
            return Error.NotFound("Wallet.NotFound", $"Wallet with id {request.WalletId} not found.");
        }

        // Aplicar cambios (el dominio controla validaciones y reglas)
        wallet.ChangeName(request.Name);
        wallet.ChangeLastName(request.LastName);
        wallet.ChangeDocument(documentType, request.DocumentNumber);
        wallet.ChangeEmail(request.Email);
        wallet.ChangePhone(request.Phone);
        // Actualizar moneda y límite a través de WalletLimit
        wallet.Limit.ChangeCurrency(currency);
        wallet.Limit.ChangeLimit(request.DailyLimit);

        await _walletRepository.UpdateAsync(wallet);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return wallet.Id.Value;
    }
}