﻿// csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WalletService.Domain.Entities;

namespace WalletService.Infrastructure.Persistence.Configurations
{
    public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
    {
        public void Configure(EntityTypeBuilder<Wallet> builder)
        {
            builder.ToTable("Wallet", schema: "Wallet");

            builder.HasKey(c => c.Id).HasName("PK_Wallet");

            builder.Property(c => c.Id)
                .HasConversion(new WalletIdConversion())
                .HasColumnType("uniqueidentifier")
                .ValueGeneratedNever()
                .HasColumnName("WalletId");

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnType("varchar(50)")
                .HasColumnName("Name");

            builder.Property(c => c.LastName)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnType("varchar(50)")
                .HasColumnName("LastName");

            builder.OwnsOne(c => c.Document, dr =>
            {
                dr.Property(d => d.Type)
                    .IsRequired()
                    .HasConversion<int>()
                    .HasColumnName("DocumentType")
                    .HasColumnType("int");

                dr.Property(d => d.Number)
                    .IsRequired()
                    .HasColumnName("DocumentNumber")
                    .HasColumnType("varchar(20)");
            });

            builder.Property(c => c.Email)
                .IsRequired()
                .HasConversion(v => v!.Address, v => Email.Create(v))
                .HasColumnName("Email")
                .HasColumnType("varchar(100)");

            builder.Property(c => c.Phone)
                .IsRequired()
                .HasConversion(v => v!.Number, v => PhoneNumber.Create(v))
                .HasColumnName("Phone")
                .HasColumnType("varchar(15)");

            builder.Property(c => c.WalletStatus)
                .IsRequired()
                .HasConversion<int>()
                .HasColumnType("int")
                .HasColumnName("WalletStatus");

            builder.Property(c => c.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime")
                .HasColumnName("CreatedAt");

            builder.Property(c => c.LastModifiedAt)
                .IsRequired()
                .HasColumnType("datetime")
                .HasColumnName("LastModifiedAt");

            // Relación con WalletLimit
            builder.HasOne(w => w.Limit)
                .WithOne(l => l.Wallet)
                .HasForeignKey<WalletLimit>(l => l.WalletId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_WalletLimit_Wallet");
        }
    }
}
