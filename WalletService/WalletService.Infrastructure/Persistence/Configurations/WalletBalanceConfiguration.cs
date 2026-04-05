using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WalletService.Infrastructure.Persistence.Configurations
{
    public class WalletBalanceConfiguration : IEntityTypeConfiguration<WalletBalance>
    {
        public void Configure(EntityTypeBuilder<WalletBalance> builder)
        {
            builder.ToTable("WalletBalance", schema: "Wallet");

            builder.HasKey(wl => wl.Id).HasName("PK_WalletBalance");

            builder.Property(wl => wl.Id)
                .HasConversion(new WalletBalanceIdConversion())
                .HasColumnType("uniqueidentifier")
                .ValueGeneratedNever()
                .HasColumnName("WalletBalanceId");

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

            builder.Property(wl => wl.BalanceAmount)
                .IsRequired()
                .HasColumnType("decimal(18,2)")
                .HasColumnName("BalanceAmount");

            
            builder.HasOne(wl => wl.Wallet)
                .WithOne(w => w.WalletBalance)
                .HasForeignKey<WalletBalance>(wl => wl.WalletId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_WalletBalance_Wallet");
        }
    }
}