using System.Net.Http.Json;
using TransactionService.Application.Abstractions.Services;
using TransactionService.Application.Transactions.Dtos;
using TransactionService.Application.Wallets.Dtos;
using TransactionService.Domain.Enums;

namespace TransactionService.Infrastructure.Services;

public class ExchangeReadService:IExcnangeReadService
{
    private readonly HttpClient _httpClient;
    
    public ExchangeReadService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<ExchangeDto?> GetByCurrencyTypeAsync(CurrencyType currencyType, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync($"/convert/currency/{currencyType}", cancellationToken);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<ExchangeDto?>(cancellationToken);
    }
}