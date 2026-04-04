using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace WalletService.Infrastructure.Persistence.Configurations;

public class WalletLimitIdConversion() : ValueConverter<WalletLimitId, Guid>(id => id.Value,
    value => new WalletLimitId(value));