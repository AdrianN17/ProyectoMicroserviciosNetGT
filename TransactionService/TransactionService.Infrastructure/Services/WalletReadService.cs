using System.Net.Http.Json;
using TransactionService.Application.Abstractions.Services;
using TransactionService.Application.Wallets.Dtos;

namespace TransactionService.Infrastructure.Services;

public class WalletReadService:IWalletReadService
{
    private readonly HttpClient _httpClient;
    
    public WalletReadService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<WalletDto?> GetByIdAsync(Guid walletId, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync($"/api/Wallets/{walletId}", cancellationToken);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<WalletDto?>(cancellationToken);
    }
}