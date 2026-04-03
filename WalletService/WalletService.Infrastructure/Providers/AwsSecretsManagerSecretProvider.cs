using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using WalletService.Application.Abstractions.Secrets;
using WalletService.Infrastructure.Caching;
using WalletService.Infrastructure.Configuration;
using System.Text.Json;

namespace WalletService.Infrastructure.Providers
{
    public sealed class AwsSecretsManagerSecretProvider : ISecretProvider
    {
        private readonly IAmazonSecretsManager _secretsManager;
        private readonly SecretsManagerOptions _options;
        private readonly InMemorySecretCache _cache;
        private readonly SemaphoreSlim _lock = new(1, 1);

        public AwsSecretsManagerSecretProvider(IAmazonSecretsManager secretsManager, SecretsManagerOptions options, InMemorySecretCache cache)
        {
            _secretsManager = secretsManager;
            _options = options;
            _cache = cache;
        }

        public async Task<IReadOnlyDictionary<string, string>> GetAllSecretsAsync(CancellationToken cancellationToken = default)
        {
            var cached = _cache.Get();
            if (cached is not null)
                return cached;

            await _lock.WaitAsync(cancellationToken);

            try
            {
                cached = _cache.Get();
                if (cached is not null)
                    return cached;

                var response = await _secretsManager.GetSecretValueAsync(new GetSecretValueRequest
                {
                    SecretId = _options.SecretName
                }, cancellationToken);

                if (response.SecretString is null)
                    throw new InvalidOperationException("Secret string is null");

                var secrets = JsonSerializer.Deserialize<Dictionary<string, string>>(response.SecretString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new Dictionary<string, string>();

                _cache.Set(secrets);

                return secrets;
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task<string?> GetSecretAsync(string secretKey, CancellationToken cancellationToken = default)
        {
            var all = await GetAllSecretsAsync(cancellationToken);
            all.TryGetValue(secretKey, out var secretValue);
            return secretValue ?? null;
        }
    }
}
