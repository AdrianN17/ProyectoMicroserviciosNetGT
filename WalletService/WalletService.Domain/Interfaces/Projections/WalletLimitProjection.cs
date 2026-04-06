namespace WalletService.Domain.Interfaces.Projections;

public record WalletLimitProjection(
    string Currency,
    decimal DailyLimit
)
{
}