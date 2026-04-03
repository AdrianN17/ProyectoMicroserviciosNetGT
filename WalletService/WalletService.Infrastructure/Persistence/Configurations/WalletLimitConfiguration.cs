using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WalletService.Infrastructure.Persistence.Configurations
{
    public class WalletLimitConfiguration : IEntityTypeConfiguration<WalletLimit>
    {
        public void Configure(EntityTypeBuilder<WalletLimit> builder)
        {
            builder.ToTable("WalletLimit", schema: "Wallet");

            builder.HasKey(wl => wl.Id).HasName("PK_WalletLimit");

            builder.Property(wl => wl.Id)
                .HasConversion(new WalletLimitIdConversion())
                .HasColumnType("uniqueidentifier")
                .ValueGeneratedNever()
                .HasColumnName("WalletLimitId");

            builder.Property(wl => wl.WalletId)
                .IsRequired()
                .HasConversion(new WalletIdConversion())
                .HasColumnType("uniqueidentifier")
                .HasColumnName("WalletId");

            builder.Property(wl => wl.Currency)
                .IsRequired()
                .HasConversion<int>()
                .HasColumnType("int")
                .HasColumnName("Currency");

            builder.Property(wl => wl.DailyLimit)
                .IsRequired()
                .HasColumnType("decimal(18,2)")
                .HasColumnName("DailyLimit");

            
            builder.HasOne(wl => wl.Wallet)
                .WithOne(w => w.Limit)
                .HasForeignKey<WalletLimit>(wl => wl.WalletId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_WalletLimit_Wallet");
        }
    }
}