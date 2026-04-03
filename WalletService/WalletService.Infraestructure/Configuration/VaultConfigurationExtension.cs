
using CustomerService.Application.Abstractions.Secrets;
using CustomerService.Infrastructure.Caching;
using CustomerService.Infrastructure.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;

namespace CustomerService.Infrastructure.Configuration
{
    public static class VaultConfigurationExtension
    {
        public static IServiceCollection AddVaultConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var vaultOptions = configuration.GetSection(VaultOptions.SectionName).Get<VaultOptions>() ?? new VaultOptions();

            if (string.IsNullOrEmpty(vaultOptions.Address)) throw new InvalidOperationException("Vault address is not configured.");
            if (string.IsNullOrEmpty(vaultOptions.Token)) throw new InvalidOperationException("Vault token is not configured.");
            if (string.IsNullOrEmpty(vaultOptions.KvMountPoint)) throw new InvalidOperationException("Vault KV mount point is not configured.");
            if (string.IsNullOrEmpty(vaultOptions.SecretPath)) throw new InvalidOperationException("Vault secret path is not configured.");

            // Initialize Vault client with token authentication and the provided address only development purposes, for production consider using more secure authentication methods
            var authMethod = new TokenAuthMethodInfo(vaultOptions.Token); 
            var vaultClientSettings = new VaultClientSettings(vaultOptions.Address, authMethod);
            IVaultClient vaultClient = new VaultClient(vaultClientSettings);

   
            services.AddSingleton(vaultOptions);
            services.AddSingleton(vaultClient);
            //services.AddSingleton<IVaultClient>(vaultClient);
            services.AddSingleton<ISecretProvider, VaultSecretProvider>();

            return services;
        }
    }
}
