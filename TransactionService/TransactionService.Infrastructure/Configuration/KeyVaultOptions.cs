namespace TransactionService.Infrastructure.Configuration
{
    public sealed class KeyVaultOptions
    {
        public const string SectionName = "KeyVault";
        public string VaultUrl { get; set; } = string.Empty;
    }
}