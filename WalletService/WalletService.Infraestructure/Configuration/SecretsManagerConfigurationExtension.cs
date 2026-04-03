using Amazon.SecretsManager;
using CustomerService.Application.Abstractions.Secrets;
using CustomerService.Infrastructure.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerService.Infrastructure.Configuration
{
    public static class SecretsManagerConfigurationExtension
    {
        public static IServiceCollection AddSecretsManagerConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var secretsManagerOptions = configuration.GetSection(SecretsManagerOptions.SectionName).Get<SecretsManagerOptions>() ?? new SecretsManagerOptions();
            if (string.IsNullOrEmpty(secretsManagerOptions.SecretName)) throw new InvalidOperationException("SecretsManager secret name is not configured.");

            services.AddSingleton(secretsManagerOptions);
            services.AddAWSService<IAmazonSecretsManager>();
            services.AddSingleton<ISecretProvider, AwsSecretsManagerSecretProvider>();

            return services;
        }
    }
}
