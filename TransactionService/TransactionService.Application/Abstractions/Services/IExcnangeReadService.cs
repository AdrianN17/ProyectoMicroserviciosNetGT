using TransactionService.Application.Transactions.Dtos;

namespace TransactionService.Application.Abstractions.Services;

public interface IExcnangeReadService
{
    Task<ExchangeDto?> GetByCurrencyTypeAsync(CurrencyType currencyType, CancellationToken cancellationToken);
}