using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TransactionService.Application.Abstractions.Secrets;
using TransactionService.Infrastructure.Caching;
using TransactionService.Infrastructure.Providers;

namespace TransactionService.Infrastructure.Configuration;

public static class KeyVaultManagerConfigurationExtension
{
    public static IServiceCollection AddKeyVaultConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var keyVaultOptions = configuration.GetSection(KeyVaultOptions.SectionName).Get<KeyVaultOptions>() ?? new KeyVaultOptions();
        if (string.IsNullOrEmpty(keyVaultOptions.VaultUrl)) throw new InvalidOperationException("KeyVault secret name is not configured.");
        
        services.AddSingleton(keyVaultOptions);
        services.AddSingleton<InMemorySecretCache>();
        services.AddSingleton<ISecretProvider, KeyVaultManagerSecretProvider>();

        return services;
    }
}