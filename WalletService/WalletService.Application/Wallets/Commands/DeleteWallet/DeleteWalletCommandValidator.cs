using FluentValidation;

namespace WalletService.Application.Wallets.Commands.DeleteWallet;

public class DeleteWalletCommandValidator : AbstractValidator<DeleteWalletCommand>
{
    private readonly IWalletRepository _walletRepository;

    public DeleteWalletCommandValidator(IWalletRepository walletRepository)
    {
        _walletRepository = walletRepository;

        RuleFor(x => x.WalletId)
            .NotEmpty().WithMessage("El identificador es requerido.");
    }
}