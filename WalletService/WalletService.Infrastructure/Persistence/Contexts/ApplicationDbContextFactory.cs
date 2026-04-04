using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WalletService.Infrastructure.Persistence.Contexts
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");

            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("DB_CONNECTION not configured");

            optionsBuilder
                .UseSqlServer(connectionString)
                .EnableSensitiveDataLogging();

            return new ApplicationDbContext(optionsBuilder.Options, new NoOpPublisher());
        }
    }

    public sealed class NoOpPublisher : IPublisher
    {
        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification => Task.CompletedTask;

        public Task Publish(object notification, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
