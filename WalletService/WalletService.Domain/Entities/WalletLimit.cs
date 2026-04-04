namespace WalletService.Domain.Entities;

using WalletService.Domain.Enums;

public sealed class WalletLimit : AuditableEntity<WalletLimitId>
{
    public WalletId WalletId { get; private set; }
    public Wallet Wallet { get; private set; } = default!;
    public CurrencyType Currency { get; private set; }
    public decimal DailyLimit { get; private set; }
    
    private WalletLimit() { }
    
    public static WalletLimit Create(WalletId walletId, CurrencyType currency, 
        decimal dailyLimit)
    {
        var errors_fields = ValidateFieldsRequired(walletId, currency, dailyLimit);
        if (errors_fields.Count > 0)
        {
            throw new DomainValidationException(
                code: "walletLimit.required_fields",
                message: "Reglas de negocio no cumplidas.",
                errors: errors_fields);
        }

        var errors = new Dictionary<string, string[]>();
        var walletLimit = new WalletLimit();
        try
        {
            walletLimit = new WalletLimit()
            {
                Id = WalletLimitId.NewId(),
                WalletId = walletId,
                Currency = currency,
                DailyLimit = dailyLimit
            };
        }
        catch (InvalidValueObjectException iv)
        {
            var prefix = iv.Code switch
            {
                "currency.invalid" => "currency",
                _ => ""
            };

            var errorsVo = iv.Errors.ToDictionary(k => $"{prefix}", v => v.Value);

            foreach (var kv in errorsVo)
                errors[kv.Key] = kv.Value;
                
        }

        if (errors.Count > 0)
            throw new DomainValidationException("wallet.invalid", "Domain validation failed", errors);

        return walletLimit;
    }

    private static Dictionary<string, string[]> ValidateFieldsRequired(WalletId walletId, CurrencyType currency, decimal dailyLimit)
    {
        var errors = new Dictionary<string, string[]>();

        if (walletId.Value == Guid.Empty)
            errors["walletId"] = new[] { "El identificador de la wallet es requerido." };
    
        if (!Enum.IsDefined(typeof(CurrencyType), currency) || currency.Equals(default(CurrencyType)))
            errors["currency"] = new[] { "El tipo de moneda es requerido y debe ser válido." };

        if (dailyLimit <= 0m)
            errors["dailyLimit"] = new[] { "El límite diario debe ser mayor a 0." };

        if (dailyLimit > 0m && (dailyLimit % 500m) != 0m)
        {
            if (errors.ContainsKey("dailyLimit"))
                errors["dailyLimit"] = errors["dailyLimit"].Concat(new[] { "El límite diario debe ser múltiplo de 500." }).ToArray();
            else
                errors["dailyLimit"] = new[] { "El límite diario debe ser múltiplo de 500." };
        }
    
        return errors;
    }
    
    public void ChangeLimit( decimal dailyLimit)
    {
        if (dailyLimit <= 0) throw new BusinessRuleViolationException("wallet.limit.invalid", "El límite debe ser un valor positivo.");
        if ((dailyLimit % 500m) != 0m) throw new BusinessRuleViolationException("wallet.limit.invalid", "El límite debe ser múltiplo de 500.");
        DailyLimit = dailyLimit;
        SetModified();
    }

    public void ChangeCurrency(CurrencyType currency)
    {
        if (!Enum.IsDefined(typeof(CurrencyType), currency) || currency.Equals(default(CurrencyType)))
            throw new BusinessRuleViolationException("wallet.currency.invalid", "El tipo de moneda es inválido.");
        Currency = currency;
        SetModified();
    }
    
    public void softDelete()
    {
        SetDeleted();
    }
}