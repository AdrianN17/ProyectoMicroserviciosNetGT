namespace WalletService.Application.Abstractions.Secrets
{
    public interface IVaultSecretProvider
    {
        Task<IDictionary<string, string>> GetSecretAsync(string secretPath);
        Task<string> GetSecretValueAsync(string secretPath, string secretKey);
    }
}
