using TransactionService.Domain.Enums;

namespace TransactionService.Domain.ValueObjects;

public sealed class Amount
{
    public decimal Value { get; private init; }
    public CurrencyType Currency { get; private init; } 
    public decimal ExchangeRate { get; private init; }
    private Amount() { }

    public static Amount Create(decimal value, CurrencyType currency, decimal exchangeRate)
    {
        if (value <= 0) throw new ArgumentException("El monto debe ser mayor a cero.", nameof(value));
        if (exchangeRate <= 0) throw new ArgumentException("La tasa de cambio debe ser mayor a cero.", nameof(exchangeRate));

        return new Amount() { Value = value, Currency = currency, ExchangeRate = exchangeRate};
    }

    public decimal ApplyExchange()
    {
        return Value * ExchangeRate;
    }

     public override string ToString() => $"{Value} {Currency:F2}";
}