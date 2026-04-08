namespace WalletService.Domain.Entities;

public sealed class WalletBalance : AuditableEntity<WalletBalanceId>
{
    public WalletId WalletId { get; private set; }
    public Wallet Wallet { get; private set; } = default!;
    public CurrencyType Currency { get; private set; }
    public decimal BalanceAmount { get; private set; }
    
    private WalletBalance() { }
    
    public static WalletBalance Create(WalletId walletId, CurrencyType currency, 
        decimal balanceAmount)
    {
        var errors_fields = ValidateFieldsRequired(walletId, currency, balanceAmount);
        if (errors_fields.Count > 0)
        {
            throw new DomainValidationException(
                code: "walletBalance.required_fields",
                message: "Reglas de negocio no cumplidas.",
                errors: errors_fields);
        }

        var errors = new Dictionary<string, string[]>();
        var walletLimit = new WalletBalance();
        try
        {
            walletLimit = new WalletBalance()
            {
                Id = WalletBalanceId.NewId(),
                WalletId = walletId,
                Currency = currency,
                BalanceAmount = balanceAmount
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

        return errors.Count > 0 ? throw new DomainValidationException("wallet.invalid", "Domain validation failed", errors) : walletLimit;
    }

    private static Dictionary<string, string[]> ValidateFieldsRequired(WalletId walletId, CurrencyType currency, decimal balanceAmount)
    {
        var errors = new Dictionary<string, string[]>();

        if (walletId.Value == Guid.Empty)
            errors["walletId"] = ["El identificador de la wallet es requerido."];
    
        if (!Enum.IsDefined(typeof(CurrencyType), currency) || currency.Equals(default(CurrencyType)))
            errors["currency"] = ["El tipo de moneda es requerido y debe ser válido."];
        
        if (balanceAmount < 0m)
            errors["balanceAmount"] = ["El balance debe ser mayor o igual a 0."];
    
        return errors;
    }
    
    public void UpdateBalance(Operation operation)
    {
        if (operation == null) throw new ArgumentNullException(nameof(operation));

        var operationCurrency = operation.Currency;

        if (operationCurrency != this.Currency)
            throw new InvalidOperationException($"La moneda de la operación '{operationCurrency}' no coincide con la moneda del balance '{this.Currency}'.");

        decimal newBalance = operation.Type switch
        {
            TypeOperation.Addition => BalanceAmount + operation.Amount,
            TypeOperation.Subtract => BalanceAmount - operation.Amount,
            _ => throw new InvalidOperationException($"Tipo de operación '{operation.Type}' no es válido.")
        };

        if (newBalance < 0m)
            throw new InvalidOperationException("El balance no puede ser negativo.");

        BalanceAmount = newBalance;
    }
}