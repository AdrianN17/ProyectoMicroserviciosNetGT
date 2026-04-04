using TransactionService.Domain.Interfaces;

namespace TransactionService.Application.Transactions.Commands.CreateTransaction;

using FluentValidation;

public class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
{
    private readonly ITransactionRepository _transactionRepository;
    
    public CreateTransactionCommandValidator(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;

        RuleFor(x => x.FromWalletId)
            .NotEmpty().WithMessage("El id del emisor es requerido.");
        
        RuleFor(x => x.ToWalletId)
            .NotEmpty().WithMessage("El id del receptor es requerido.");
        
        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("La moneda es requerida.");
        
        RuleFor(x => x.SourceType)
            .NotEmpty().WithMessage("El origen es requerida.");
        
        RuleFor(x => x.Amount)
            .NotEmpty().WithMessage("El monto es requerido.")
            .InclusiveBetween(1m, 500m).WithMessage("El monto debe ser mayor o igual a 1 y menor o igual a 500.");

    }
}