
namespace CustomerService.Infrastructure.Configuration
{
    public sealed class SecretsManagerOptions
    {
        public const string SectionName = "SecretsManager";
        public string SecretName { get; set; } = string.Empty;
    }
}
