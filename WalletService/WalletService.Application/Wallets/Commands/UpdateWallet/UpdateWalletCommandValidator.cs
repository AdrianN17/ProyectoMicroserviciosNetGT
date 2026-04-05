using FluentValidation;

namespace WalletService.Application.Wallets.Commands.UpdateWallet;

public class UpdateWalletCommandValidator : AbstractValidator<UpdateWalletCommand>
{
    private readonly IWalletRepository _walletRepository;

    public UpdateWalletCommandValidator(IWalletRepository walletRepository)
    {
        _walletRepository = walletRepository;

        // Validación global: al menos un campo debe ser enviado
        RuleFor(x => x)
            .Must(x => x.Name != null || x.LastName != null || x.DocumentType != null ||
                       x.DocumentNumber != null || x.Email != null || x.Phone != null ||
                       x.DailyLimit != null)
            .WithMessage("Debe enviar al menos un campo para actualizar.")
            .OverridePropertyName("request");

        // Name: solo valida si viene con valor
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre no puede ser vacío.")
            .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.")
            .When(x => x.Name != null);

        // LastName: solo valida si viene con valor
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("El apellido no puede ser vacío.")
            .MaximumLength(100).WithMessage("El apellido no puede exceder los 100 caracteres.")
            .When(x => x.LastName != null);

        // DocumentType: solo valida si viene con valor
        RuleFor(x => x.DocumentType)
            .NotEmpty().WithMessage("El tipo de documento no puede ser vacío.")
            .Must(value => EnumParsing.TryParseEnum<DocumentType>(value, out _))
            .WithMessage("El tipo de documento no es válido.")
            .When(x => x.DocumentType != null);

        // DocumentNumber: solo valida si viene con valor
        RuleFor(x => x.DocumentNumber)
            .NotEmpty().WithMessage("El número de documento no puede ser vacío.")
            .MaximumLength(20).WithMessage("El número de documento no puede exceder los 20 caracteres.")
            .MustAsync(async (docNumber, cancellationToken) =>
                !await _walletRepository.ExistsByDocumentNumberAsync(docNumber!, cancellationToken))
            .WithMessage("El número de documento ya existe.")
            .When(x => x.DocumentNumber != null);

        // Validación cruzada: solo cuando ambos vienen en la request
        RuleFor(x => x)
            .Must(x => IsValidDocumentNumber(x.DocumentNumber!, x.DocumentType!))
            .WithMessage("El número de documento no es válido para el tipo de documento especificado.")
            .OverridePropertyName("documentNumber")
            .When(x => x.DocumentType != null && x.DocumentNumber != null);

        // Email: solo valida si viene con valor
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El email no puede ser vacío.")
            .EmailAddress().WithMessage("El email no es válido.")
            .When(x => x.Email != null);

        // Phone: solo valida si viene con valor
        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("El teléfono no puede ser vacío.")
            .Matches(@"^[+]?[0-9\-\s]{7,15}$").WithMessage("El teléfono no tiene un formato válido.")
            .When(x => x.Phone != null);

        // DailyLimit: solo valida si viene con valor
        RuleFor(x => x.DailyLimit)
            .GreaterThan(0m).WithMessage("El límite diario debe ser mayor a 0.")
            .Must(d => (d % 500m) == 0m).WithMessage("El límite diario debe ser múltiplo de 500.")
            .When(x => x.DailyLimit != null);
    }

    private bool IsValidDocumentNumber(string documentNumber, string documentType)
    {
        if (!EnumParsing.TryParseEnum<DocumentType>(documentType, out var docType))
            return false;
        return DocumentRules.IsValid(docType, documentNumber.Trim());
    }
}