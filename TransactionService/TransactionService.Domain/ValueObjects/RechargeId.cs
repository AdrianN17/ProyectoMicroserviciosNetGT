namespace TransactionService.Domain.ValueObjects;

public readonly record struct RechargeId(Guid Value)
{
    public static RechargeId NewId() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}