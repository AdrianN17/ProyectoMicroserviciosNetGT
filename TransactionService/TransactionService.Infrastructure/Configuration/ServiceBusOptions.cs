namespace TransactionService.Infrastructure.Configuration;

public sealed class ServiceBusOptions
{
    public const string SectionName = "ServiceBus";
    public string QueueName { get; set; } = string.Empty;
}