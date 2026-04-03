namespace WalletService.Domain.Common;

public record WalletLimit
{
    public WalletLimitId WalletLimitId { get; init; } = default!;
    public CurrencyType Currency { get; init; }
    public decimal DailyLimit { get; init; }
    
    
    public static WalletLimit Create(CurrencyType currency, decimal dailyLimit, WalletLimitId? walletLimitId = null)
    {
        var errors = new Dictionary<string, string[]>();

        if (!Enum.IsDefined(typeof(CurrencyType), currency) || currency.Equals(default(CurrencyType)))
            errors["currency"] = new[] { "El tipo de moneda es requerido y debe ser válido." };

        if (dailyLimit <= 0m)
            errors["dailyLimit"] = new[] { "El límite diario debe ser mayor a 0." };

        // Validación adicional: debe ser múltiplo de 500
        if (dailyLimit > 0m && (dailyLimit % 500m) != 0m)
        {
            // Si ya existe un error en dailyLimit, lo preservamos y añadimos detalle; en caso contrario creamos uno nuevo
            if (errors.ContainsKey("dailyLimit"))
                errors["dailyLimit"] = errors["dailyLimit"].Concat(new[] { "El límite diario debe ser múltiplo de 500." }).ToArray();
            else
                errors["dailyLimit"] = new[] { "El límite diario debe ser múltiplo de 500." };
        }

        if (errors.Any())
            throw new ValidationException(errors);

        return new WalletLimit
        {
            WalletLimitId = walletLimitId ?? new WalletLimitId(Guid.NewGuid().ToString()),
            Currency = currency,
            DailyLimit = dailyLimit
        };
    }
}