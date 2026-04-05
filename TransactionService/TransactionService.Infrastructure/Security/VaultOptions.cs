
namespace TransactionService.Infrastructure.Security
{
    public sealed class VaultOptions
    {
        public string Address { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string MountPoint { get; set; } = "secret";
        public string? SecretPath { get; set; } = string.Empty;
    }
}
