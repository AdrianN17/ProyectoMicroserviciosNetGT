
namespace WalletService.Application.Commmon.Interfaces
{
    public interface IApplicationDbContext
    {
        // DbSet<Customer> Customers { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
