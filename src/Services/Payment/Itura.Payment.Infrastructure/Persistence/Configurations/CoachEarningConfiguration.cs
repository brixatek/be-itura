using Itura.Payment.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itura.Payment.Infrastructure.Persistence.Configurations;

internal sealed class CoachEarningConfiguration : IEntityTypeConfiguration<CoachEarning>
{
    public void Configure(EntityTypeBuilder<CoachEarning> builder)
    {
        builder.ToTable("coach_earnings");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(e => e.CoachUserId).HasColumnName("coach_user_id").IsRequired();
        builder.Property(e => e.SessionId).HasColumnName("session_id").IsRequired();
        builder.Property(e => e.BookingId).HasColumnName("booking_id").IsRequired();
        builder.Property(e => e.GrossAmount).HasColumnName("gross_amount").HasPrecision(18, 2).IsRequired();
        builder.Property(e => e.CommissionRate).HasColumnName("commission_rate").HasPrecision(5, 4).IsRequired();
        builder.Property(e => e.CommissionAmount).HasColumnName("commission_amount").HasPrecision(18, 2).IsRequired();
        builder.Property(e => e.NetAmount).HasColumnName("net_amount").HasPrecision(18, 2).IsRequired();
        builder.Property(e => e.Currency).HasColumnName("currency").HasMaxLength(3).IsRequired();
        builder.Property(e => e.IsPaid).HasColumnName("is_paid").HasDefaultValue(false);
        builder.Property(e => e.PayoutId).HasColumnName("payout_id");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at");

        builder.HasIndex(e => e.CoachUserId).HasDatabaseName("ix_coach_earnings_coach_user_id");
        builder.HasIndex(e => new { e.CoachUserId, e.IsPaid }).HasDatabaseName("ix_coach_earnings_coach_unpaid");
    }
}
