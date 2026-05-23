using Itura.Payment.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itura.Payment.Infrastructure.Persistence.Configurations;

internal sealed class WalletTransactionConfiguration : IEntityTypeConfiguration<WalletTransaction>
{
    public void Configure(EntityTypeBuilder<WalletTransaction> builder)
    {
        builder.ToTable("wallet_transactions");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(t => t.WalletId).HasColumnName("wallet_id").IsRequired();
        builder.Property(t => t.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(t => t.Amount).HasColumnName("amount").HasPrecision(18, 2).IsRequired();
        builder.Property(t => t.Type).HasColumnName("type").HasMaxLength(10).IsRequired();
        builder.Property(t => t.Description).HasColumnName("description").HasMaxLength(500).IsRequired();
        builder.Property(t => t.Reference).HasColumnName("reference").HasMaxLength(200);
        builder.Property(t => t.BalanceAfter).HasColumnName("balance_after").HasPrecision(18, 2);
        builder.Property(t => t.CreatedAt).HasColumnName("created_at");

        builder.HasIndex(t => t.UserId).HasDatabaseName("ix_wallet_transactions_user_id");
        builder.HasIndex(t => t.WalletId).HasDatabaseName("ix_wallet_transactions_wallet_id");
    }
}
