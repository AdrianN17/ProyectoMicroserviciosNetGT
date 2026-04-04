
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace WalletService.Infrastructure.Persistence.Configurations
{
    public class WalletIdConversion() : ValueConverter<WalletId, Guid>(id => id.Value,
        value => new WalletId(value));
}
