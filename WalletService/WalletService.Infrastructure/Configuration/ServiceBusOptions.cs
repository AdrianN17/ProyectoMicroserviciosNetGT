namespace WalletService.Infrastructure.Configuration;

public class ServiceBusOptions
{
    public const string SectionName = "ServiceBus";
    public string QueueName { get; set; }
}