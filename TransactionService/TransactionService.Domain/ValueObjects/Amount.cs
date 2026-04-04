using WalletService.Domain.Enums;

namespace TransactionService.Domain.ValueObjects;

public sealed class Amount
{
    public decimal Value { get; private init; }
    public CurrencyType Currency { get; private init; } 
    private Amount() { }

    public static Amount Create(decimal value, CurrencyType currency)
    {
        if (value <= 0) throw new ArgumentException("El monto debe ser mayor a cero.", nameof(value));
        if (value > 500) throw new ArgumentException("El monto debe ser menor o igual a 500.", nameof(value));
        
        return new Amount() { Value = value, Currency = currency };
    }

     public override string ToString() => $"{Value} {Currency:F2}";
}