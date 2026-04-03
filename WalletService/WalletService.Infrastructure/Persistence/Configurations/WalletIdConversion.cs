
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace WalletService.Infrastructure.Persistence.Configurations
{
    public class WalletIdConversion : ValueConverter<WalletId, Guid>
    {
        public WalletIdConversion() : base(
            id => id.Value, 
            value => new WalletId(value)) 
        {
        }
    }

    public class WalletLimitIdConversion : ValueConverter<WalletLimitId, Guid>
    {
        public WalletLimitIdConversion() : base(
            id => id.Value,
            value => new WalletLimitId(value))
        {
        }
    }
}
