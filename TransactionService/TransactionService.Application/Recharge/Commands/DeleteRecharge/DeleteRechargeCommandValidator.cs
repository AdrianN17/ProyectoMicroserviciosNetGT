using FluentValidation;

namespace TransactionService.Application.Transactions.Commands.DeleteRecharge;

public class DeleteRechargeCommandValidator : AbstractValidator<DeleteRechargeCommand>
{
    public DeleteRechargeCommandValidator()
    {
        RuleFor(x => x.RechargeId)
            .NotEmpty().WithMessage("El identificador de la recarga es requerido.");
    }
}

