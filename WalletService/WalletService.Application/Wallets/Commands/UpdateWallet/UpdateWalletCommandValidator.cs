using FluentValidation;

namespace WalletService.Application.Wallets.Commands.UpdateWallet;

public class UpdateWalletCommandValidator : AbstractValidator<UpdateWalletCommand>
{
    private readonly IWalletRepository _walletRepository;

    public UpdateWalletCommandValidator(IWalletRepository walletRepository)
    {
        _walletRepository = walletRepository;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("El apellido es requerido.")
            .MaximumLength(100).WithMessage("El apellido no puede exceder los 100 caracteres.");

        RuleFor(x => x.DocumentType)
            .NotEmpty().WithMessage("El tipo de documento es requerido.")
            .Must(value => EnumParsing.TryParseEnum<DocumentType>(value, out _))
            .WithMessage("El tipo de documento no es válido.");
        
        RuleFor(x => x.DocumentNumber)
            .NotEmpty().WithMessage("El número de documento es requerido.")
            .MaximumLength(20).WithMessage("El número de documento no puede exceder los 50 caracteres.")
            .MustAsync(async (docNumber, cancellationToken) => !await BeUniqueDocumentNumber(docNumber, cancellationToken)).WithMessage("El número de documento ya existe.");

        RuleFor(x => x.DocumentType)
            .NotEmpty().WithMessage("El tipo de documento es requerido")
            .MustAsync(IsNotValidDocumentType).WithMessage("El tipo de documento no es válido.");
        
        RuleFor(x => x)
            .Must((x) => IsValidDocumentNumber(x.DocumentNumber, x.DocumentType)).WithMessage("El número de documento no es válido para el tipo de documento especificado.")
            .OverridePropertyName($"documentNumber");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email es requerido.")
            .EmailAddress().WithMessage("El email no es válido.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("El teléfono es requerido.")
            .Matches(@"^[+]?[0-9\-\s]{7,15}$").WithMessage("El teléfono no tiene un formato válido.");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("La moneda es requerida.")
            .Must(value => EnumParsing.TryParseEnum<CurrencyType>(value, out _))
            .WithMessage("La moneda no es válida.");

        RuleFor(x => x.DailyLimit)
            .GreaterThan(0m).WithMessage("El límite diario debe ser mayor a 0.")
            .Must(d => (d % 500m) == 0m).WithMessage("El límite diario debe ser múltiplo de 500.");
        
        RuleFor(x => x.DailyLimitId)
            .NotEmpty().WithMessage("El Identificador de daily limit es requerido.")
    }
    
    private async Task<bool> BeUniqueDocumentNumber(string documentNumber, CancellationToken cancellationToken)
    {
        return !await _walletRepository.ExistsByDocumentNumberAsync(documentNumber, cancellationToken);
    }

    private async Task<bool> IsNotValidDocumentType(string documentType, CancellationToken cancellationToken)
    {
        return EnumParsing.TryParseEnum<DocumentType>(documentType, out _);
    }

    private bool IsValidDocumentNumber(string documentNumber, string documentType)
    {
        if (!EnumParsing.TryParseEnum<DocumentType>(documentType, out var docType))
            return false;
        return DocumentRules.IsValid(docType, documentNumber.Trim());
    }
}