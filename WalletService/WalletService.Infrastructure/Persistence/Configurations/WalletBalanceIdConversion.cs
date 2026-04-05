using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace WalletService.Infrastructure.Persistence.Configurations;

public class WalletBalanceIdConversion() : ValueConverter<WalletBalanceId, Guid>(id => id.Value,
    value => new WalletBalanceId(value));