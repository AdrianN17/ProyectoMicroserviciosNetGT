namespace WalletService.Domain.Common;

public record DocumentId
{
    public string Number { get; init; } = default!;
    public DocumentType Type { get; init; }
    
    private DocumentId() { }

    public static DocumentId Create(DocumentType type, string number)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(number))
        {
            errors["number"] = new[] { "El número de documento es requerido" };
        }
        else
        {
            var normalizeNumber = Normalize(number);

            if (!DocumentRules.IsValid(type, normalizeNumber))
            {
                errors["number"] = new[] { $"Número de documento inválido para {type}. {DocumentRules.GetHint(type)}" };
            }

            if (errors.Count > 0)
                throw new InvalidValueObjectException(
                    code: "document.invalid",
                    message: "El documento no es válido",
                    errors: errors);

            return new DocumentId() { Number = normalizeNumber, Type = type };
        }

        // Si llegamos aquí significa que el número estaba vacío y ya hay errores
        throw new InvalidValueObjectException(
            code: "document.invalid",
            message: "El documento no es válido",
            errors: errors);
    }
    
    private static string Normalize(string number) => number.Trim().Replace("-", string.Empty).ToUpperInvariant();

    public override string ToString() => $"{Type}: {Number}";
}