namespace WalletService.Domain.Interfaces.Projections;

public record WalletLimitProjection
{
    public string Currency { get; init; }
    public decimal DailyLimit { get; init; }
}