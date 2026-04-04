using FluentValidation;
using TransactionService.Domain.Interfaces;

namespace TransactionService.Application.Transactions.Commands.DeleteTransaction;

public class DeleteTransactionCommandValidator : AbstractValidator<DeleteTransactionCommandd>
{
    private readonly ITransactionRepository _transactionRepository;

    public DeleteTransactionCommandValidator(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;

        RuleFor(x => x.TransactionId)
            .NotEmpty().WithMessage("El identificador es requerido.");
    }
}