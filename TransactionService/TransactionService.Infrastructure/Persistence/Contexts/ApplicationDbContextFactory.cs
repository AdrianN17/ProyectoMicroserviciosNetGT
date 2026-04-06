using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TransactionService.Infrastructure.Persistence.Contexts
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            var endpoint     = Environment.GetEnvironmentVariable("COSMOS_ENDPOINT")
                               ?? throw new InvalidOperationException("COSMOS_ENDPOINT not configured");
            var accountKey   = Environment.GetEnvironmentVariable("COSMOS_KEY")
                               ?? throw new InvalidOperationException("COSMOS_KEY not configured");
            var databaseName = Environment.GetEnvironmentVariable("COSMOS_DATABASE")
                               ?? "TransactionServiceCosmosDb";

            optionsBuilder
                .UseCosmos(endpoint, accountKey, databaseName)
                .EnableSensitiveDataLogging();

            return new ApplicationDbContext(optionsBuilder.Options, new NoOpPublisher());
        }
    }

    public sealed class NoOpPublisher : IPublisher
    {
        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification => Task.CompletedTask;

        public Task Publish(object notification, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }
}
