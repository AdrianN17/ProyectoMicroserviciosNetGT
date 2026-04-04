
namespace TransactionService.Domain.Common
{
    public interface IAuditable<TUser>
    {
        DateTime CreatedAt { get; set; }
        DateTime? LastModifiedAt { get; set; }
        bool IsDeleted { get; set; }
        DateTime? DeletedAt { get; set; }
    }
}
