using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TransactionService.Infrastructure.Persistence.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            // Container de Cosmos DB
            builder.ToContainer("Transactions");

            // Ignorar la colección de domain events — no debe persistirse
            builder.Ignore(t => t.DomainEvents);

            // Clave primaria — TransactionId (record struct) → string
            builder.Property(t => t.Id)
                .HasConversion(
                    id => id.Value.ToString(),
                    value => new TransactionId(Guid.Parse(value)));

            // WalletId es un readonly record struct → Guid plano en JSON
            builder.Property(t => t.FromWalletId)
                .HasConversion(
                    w => w.Value,
                    v => new WalletId(v))
                .ToJsonProperty("fromWalletId");

            builder.Property(t => t.ToWalletId)
                .HasConversion(
                    w => w.Value,
                    v => new WalletId(v))
                .ToJsonProperty("toWalletId");

            // La partition key apunta directamente a la propiedad ya mapeada
            builder.HasPartitionKey(t => t.FromWalletId);

            // Amount es clase sellada (tipo referencia) → OwnsOne
            builder.OwnsOne(t => t.Amount, vo =>
            {
                vo.Property(a => a.Value).ToJsonProperty("amount");
                vo.Property(a => a.Currency)
                    .HasConversion<string>()
                    .ToJsonProperty("currency");
            });

            // Enums como string
            builder.Property(t => t.TransactionStatus).HasConversion<string>();
            builder.Property(t => t.SourceType).HasConversion<string>();
        }
    }
}
