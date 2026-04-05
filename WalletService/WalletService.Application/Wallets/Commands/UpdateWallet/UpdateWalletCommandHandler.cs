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
        Console.WriteLine("parte handler");
        _logger.LogInformation("Updating wallet with document number {DocumentNumber}", request.DocumentNumber);
        
        var wallet = await _walletRepository.GetByIdAsync(new WalletId(request.WalletId));
        if (wallet == null)
        {
            return Error.NotFound("Wallet.NotFound", $"Wallet with id {request.WalletId} not found.");
        }

        if(request.Name != null)
            wallet.ChangeName(request.Name);
        if(request.LastName != null)
            wallet.ChangeLastName(request.LastName);


        if (request.DocumentType != null)
        {
            if (!EnumParsing.TryParseEnum<DocumentType>(request.DocumentType, out var documentType))
                return Error.Validation(code: "DocumentType.Invalid", description: $"DocumentType '{request.DocumentType}' no es válido.");

            wallet.ChangeDocument(documentType, request.DocumentNumber ?? wallet.Document.Number);
        }
        else
            wallet.ChangeDocument(wallet.Document.Type, request.DocumentNumber ?? wallet.Document.Number);
        
        
        if(request.Email != null)
            wallet.ChangeEmail(request.Email);
        
        if(request.Phone != null)
            wallet.ChangePhone(request.Phone);

        if(request.DailyLimit != null)
            wallet.WalletLimit.ChangeLimit((decimal)request.DailyLimit);

        await _walletRepository.UpdateAsync(wallet);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return wallet.Id.Value;
    }
}