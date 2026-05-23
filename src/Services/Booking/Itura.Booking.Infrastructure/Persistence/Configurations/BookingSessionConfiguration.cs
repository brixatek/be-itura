using Itura.Booking.Domain.Entities;
using Itura.Booking.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itura.Booking.Infrastructure.Persistence.Configurations;

internal sealed class BookingSessionConfiguration : IEntityTypeConfiguration<BookingSession>
{
    public void Configure(EntityTypeBuilder<BookingSession> builder)
    {
        builder.ToTable("booking_sessions");

        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(b => b.CoachId).HasColumnName("coach_id").IsRequired();
        builder.Property(b => b.CoachUserId).HasColumnName("coach_user_id").IsRequired();
        builder.Property(b => b.ClientUserId).HasColumnName("client_user_id").IsRequired();
        builder.Property(b => b.ScheduledAt).HasColumnName("scheduled_at").IsRequired();
        builder.Property(b => b.DurationMinutes).HasColumnName("duration_minutes").IsRequired();
        builder.Property(b => b.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();
        builder.Property(b => b.Price).HasColumnName("price").HasPrecision(18, 2).IsRequired();
        builder.Property(b => b.Currency).HasColumnName("currency").HasMaxLength(3).IsRequired();
        builder.Property(b => b.CancellationReason).HasColumnName("cancellation_reason").HasMaxLength(500);
        builder.Property(b => b.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(b => b.UpdatedAt).HasColumnName("updated_at");
        builder.Property(b => b.DeletedAt).HasColumnName("deleted_at");

        builder.HasQueryFilter(b => !b.IsDeleted);

        builder.HasIndex(b => b.ClientUserId).HasDatabaseName("ix_booking_sessions_client_user_id");
        builder.HasIndex(b => b.CoachId).HasDatabaseName("ix_booking_sessions_coach_id");
        builder.HasIndex(b => b.CoachUserId).HasDatabaseName("ix_booking_sessions_coach_user_id");
        builder.HasIndex(b => b.ScheduledAt).HasDatabaseName("ix_booking_sessions_scheduled_at");
        builder.HasIndex(b => b.Status).HasDatabaseName("ix_booking_sessions_status");
    }
}
