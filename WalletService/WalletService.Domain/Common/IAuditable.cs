
namespace WalletService.Domain.Common
{
    public interface IAuditable<TUser>
    {
        TUser? CreatedBy { get; set; }
        DateTime CreatedAt { get; set; }

        TUser? LastModifiedBy { get; set; }
        DateTime? LastModifiedAt { get; set; }

        bool IsDeleted { get; set; }

        TUser? DeletedBy { get; set; }
        DateTime? DeletedAt { get; set; }
    }
}
