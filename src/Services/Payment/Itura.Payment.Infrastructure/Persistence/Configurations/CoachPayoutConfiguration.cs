using Itura.Payment.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itura.Payment.Infrastructure.Persistence.Configurations;

internal sealed class CoachPayoutConfiguration : IEntityTypeConfiguration<CoachPayout>
{
    public void Configure(EntityTypeBuilder<CoachPayout> builder)
    {
        builder.ToTable("coach_payouts");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(p => p.CoachUserId).HasColumnName("coach_user_id").IsRequired();
        builder.Property(p => p.Amount).HasColumnName("amount").HasPrecision(18, 2).IsRequired();
        builder.Property(p => p.Currency).HasColumnName("currency").HasMaxLength(3).IsRequired();
        builder.Property(p => p.Status).HasColumnName("status").HasMaxLength(20).IsRequired();
        builder.Property(p => p.TransferReference).HasColumnName("transfer_reference").HasMaxLength(200);
        builder.Property(p => p.FailureReason).HasColumnName("failure_reason").HasMaxLength(500);
        builder.Property(p => p.ProcessedAt).HasColumnName("processed_at");
        builder.Property(p => p.CreatedAt).HasColumnName("created_at");
        builder.Property(p => p.UpdatedAt).HasColumnName("updated_at");

        builder.HasIndex(p => p.CoachUserId).HasDatabaseName("ix_coach_payouts_coach_user_id");
        builder.HasIndex(p => p.TransferReference).HasDatabaseName("ix_coach_payouts_transfer_reference");
    }
}
