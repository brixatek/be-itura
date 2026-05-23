using Itura.Payment.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itura.Payment.Infrastructure.Persistence.Configurations;

internal sealed class WalletConfiguration : IEntityTypeConfiguration<Wallet>
{
    public void Configure(EntityTypeBuilder<Wallet> builder)
    {
        builder.ToTable("wallets");
        builder.HasKey(w => w.Id);
        builder.Property(w => w.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(w => w.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(w => w.Balance).HasColumnName("balance").HasPrecision(18, 2).HasDefaultValue(0m);
        builder.Property(w => w.SessionCredits).HasColumnName("session_credits").HasDefaultValue(0);
        builder.Property(w => w.Currency).HasColumnName("currency").HasMaxLength(3).IsRequired();
        builder.Property(w => w.Version).HasColumnName("version").IsConcurrencyToken();
        builder.Property(w => w.CreatedAt).HasColumnName("created_at");
        builder.Property(w => w.UpdatedAt).HasColumnName("updated_at");

        builder.HasIndex(w => w.UserId).IsUnique().HasDatabaseName("ix_wallets_user_id");
    }
}
