// Ajusta namespace y tipos: usar DocumentType del proyecto y Regex
namespace WalletService.Domain.ValueObjects;

using System.Text.RegularExpressions;
using WalletService.Domain.Common;

public static class DocumentRules
{
    // Puedes convertir esto en Strategy classes si crece mucho.
    private static readonly Dictionary<DocumentType, Func<string, bool>> Validators = new()
    {
        { DocumentType.DNI,         n => Regex.IsMatch(n, @"^\d{8}$") },
        { DocumentType.CE,          n => Regex.IsMatch(n, @"^[A-Z0-9]{8,12}$") },
        { DocumentType.PASSPORT,    n => Regex.IsMatch(n, @"^[A-Z0-9]{5,15}$") },
        { DocumentType.RUC,         n => Regex.IsMatch(n, @"^\d{11}$") },
    };

    public static bool IsValid(DocumentType type, string number)
        => Validators.TryGetValue(type, out var validator) && validator(number);

    public static string GetHint(DocumentType type) => type switch
    {
        DocumentType.DNI        => "DNI debe tener 8 dígitos.",
        DocumentType.CE         => "CE debe ser alfanumérico (8 a 12).",
        DocumentType.PASSPORT   => "Pasaporte debe ser alfanumérico (5 a 15).",
        DocumentType.RUC        => "RUC debe tener 11 dígitos.",
        _ => "Tipo de documento no soportado."
    };
}