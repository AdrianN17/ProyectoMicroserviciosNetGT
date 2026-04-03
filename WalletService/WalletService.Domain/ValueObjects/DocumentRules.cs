namespace DefaultNamespace;

public static class DocumentRules
{
    // Puedes convertir esto en Strategy classes si crece mucho.
    private static readonly Dictionary<IdentityDocumentType, Func<string, bool>> Validators = new()
    {
        { IdentityDocumentType.DNI,         n => Regex.IsMatch(n, @"^\d{8}$") },
        { IdentityDocumentType.CE,          n => Regex.IsMatch(n, @"^[A-Z0-9]{8,12}$") },
        { IdentityDocumentType.PASSPORT,    n => Regex.IsMatch(n, @"^[A-Z0-9]{5,15}$") },
        { IdentityDocumentType.RUC,         n => Regex.IsMatch(n, @"^\d{11}$") },
    };

    public static bool IsValid(IdentityDocumentType type, string number)
        => Validators.TryGetValue(type, out var validator) && validator(number);

    public static string GetHint(IdentityDocumentType type) => type switch
    {
        IdentityDocumentType.DNI        => "DNI debe tener 8 dígitos.",
        IdentityDocumentType.CE         => "CE debe ser alfanumérico (8 a 12).",
        IdentityDocumentType.PASSPORT   => "Pasaporte debe ser alfanumérico (5 a 15).",
        IdentityDocumentType.RUC        => "RUC debe tener 11 dígitos.",
        _ => "Tipo de documento no soportado."
    };
}