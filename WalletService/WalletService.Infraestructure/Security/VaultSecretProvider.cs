using WalletService.Application.Abstractions.Secrets;
using VaultSharp;

namespace WalletService.Infrastructure.Security
{
    public class VaultSecretProvider : IVaultSecretProvider
    {
        private readonly IVaultClient _vaultClient;
        private readonly string _mountPoint;
        public VaultSecretProvider(IVaultClient vaultClient, string mountPoint)
        {
            _vaultClient = vaultClient;
            _mountPoint = mountPoint;
        }
        public async Task<IDictionary<string, string>> GetSecretAsync(string secretPath)
        {
            var secret = await _vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(secretPath, mountPoint: _mountPoint);
            return secret.Data.Data.ToDictionary(k => k.Key, v => v.Value?.ToString() ?? string.Empty);
        }

        public async Task<string> GetSecretValueAsync(string secretPath, string secretKey)
        {
            var allSecrets = await GetSecretAsync(secretPath);
            allSecrets.TryGetValue(secretKey, out var secretValue);
            return secretValue ?? string.Empty;
        }
    }
}
