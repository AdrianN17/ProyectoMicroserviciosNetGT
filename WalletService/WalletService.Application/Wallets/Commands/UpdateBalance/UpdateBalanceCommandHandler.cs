﻿using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;
using WalletService.Application.Commmon.Interfaces;

namespace WalletService.Application.Wallets.Commands.UpdateBalance;

public sealed class UpdateBalanceCommandHandler : IRequestHandler<UpdateBalanceCommand, ErrorOr<Guid>>
{
    private readonly IWalletRepository _walletRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateBalanceCommandHandler> _logger;

    public UpdateBalanceCommandHandler(IWalletRepository walletRepository, IUnitOfWork unitOfWork, ILogger<UpdateBalanceCommandHandler> logger)
    {
        _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ErrorOr<Guid>> Handle(UpdateBalanceCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing balance update for WalletId {WalletId}", request.WalletId);

        // Parsear enums para construir el VO Operation
        if (!EnumParsing.TryParseEnum<TypeOperation>(request.Type, out var typeOperation))
            return Error.Validation(code: "TypeOperation.Invalid", description: $"TypeOperation '{request.Type}' no es válido.");

        if (!EnumParsing.TryParseEnum<CurrencyType>(request.Currency, out var currency))
            return Error.Validation(code: "CurrencyType.Invalid", description: $"CurrencyType '{request.Currency}' no es válido.");

        // Construir el ValueObject Operation del dominio
        var operation = new Operation
        {
            Type = typeOperation,
            WalletId = new WalletId(request.WalletId),
            Amount = request.Amount,
            Currency = currency
        };

        // Cargar la wallet por Id
        var wallet = await _walletRepository.GetByIdAsync(operation.WalletId, cancellationToken);
        if (wallet is null)
            return Error.NotFound("Wallet.NotFound", $"Wallet con id '{request.WalletId}' no fue encontrada.");

        // Llamar al método del dominio en WalletBalance
        wallet.WalletBalance.UpdateBalance(operation);

        await _walletRepository.UpdateAsync(wallet, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Balance updated successfully for WalletId {WalletId}", request.WalletId);

        return wallet.Id.Value;
    }
}

