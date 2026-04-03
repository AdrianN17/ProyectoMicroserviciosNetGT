using WalletService.Application.Abstractions.Secrets;
using WalletService.Infrastructure.Caching;
using WalletService.Infrastructure.Configuration;
using VaultSharp;

namespace WalletService.Infrastructure.Providers
{
    public sealed class VaultSecretProvider : ISecretProvider
    {
        private readonly VaultOptions _options;
        private readonly IVaultClient _vaultClient;
        private readonly InMemorySecretCache _cache;

        public VaultSecretProvider(IVaultClient vaultClient, VaultOptions options, InMemorySecretCache cache)
        {
            _vaultClient = vaultClient;
            _options = options;
            _cache = cache;
        }

        public async Task<IReadOnlyDictionary<string, string>> GetAllSecretsAsync(CancellationToken cancellationToken = default)
        {
            var cached = _cache.Get();
            if (cached is not null)
                return cached;

            var secret = await _vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(path: _options.SecretPath, mountPoint: _options.KvMountPoint);
            var result = secret.Data.Data.ToDictionary(k => k.Key, v => v.Value?.ToString() ?? string.Empty, StringComparer.OrdinalIgnoreCase);
            _cache.Set(result);

            return result;
        }

        public async Task<string?> GetSecretAsync(string secretKey, CancellationToken cancellationToken = default)
        {
            var all = await GetAllSecretsAsync(cancellationToken);
            all.TryGetValue(secretKey, out var secretValue);
            return secretValue ?? null;
        }
    }
}
