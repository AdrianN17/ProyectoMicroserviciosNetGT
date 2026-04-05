using FluentValidation;

namespace TransactionService.Application.Transactions.Commands.CreateRecharge;

public class CreateRechargeCommandValidator : AbstractValidator<CreateRechargeCommand>
{
    public CreateRechargeCommandValidator()
    {
        RuleFor(x => x.WalletId)
            .NotEmpty().WithMessage("El identificador de la billetera es requerido.");

        RuleFor(x => x.Amount)
            .NotEmpty().WithMessage("El monto es requerido.")
            .GreaterThan(0m).WithMessage("El monto debe ser mayor a cero.");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("La moneda es requerida.");

        RuleFor(x => x.MethodType)
            .NotEmpty().WithMessage("El método de recarga es requerido.");
    }
}

