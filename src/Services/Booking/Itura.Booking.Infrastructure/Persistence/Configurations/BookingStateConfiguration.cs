using Itura.Booking.Infrastructure.Sagas;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Itura.Booking.Infrastructure.Persistence.Configurations;

internal sealed class BookingStateConfiguration : SagaClassMap<BookingState>
{
    protected override void Configure(EntityTypeBuilder<BookingState> entity, ModelBuilder model)
    {
        entity.ToTable("booking_saga_states", "itura_booking");

        entity.HasKey(e => e.CorrelationId);
        entity.Property(e => e.CorrelationId).HasColumnName("correlation_id");
        entity.Property(e => e.CurrentState).IsRequired().HasMaxLength(64).HasColumnName("current_state");
        entity.Property(e => e.CoachId).HasColumnName("coach_id");
        entity.Property(e => e.CoachUserId).HasColumnName("coach_user_id");
        entity.Property(e => e.ClientUserId).HasColumnName("client_user_id");
        entity.Property(e => e.ScheduledAt).HasColumnName("scheduled_at");
        entity.Property(e => e.Price).HasPrecision(18, 2).HasColumnName("price");
        entity.Property(e => e.Currency).IsRequired().HasMaxLength(3).HasColumnName("currency");
        entity.Property(e => e.CancellationReason).HasMaxLength(500).HasColumnName("cancellation_reason");
        entity.Property(e => e.CreatedAt).HasColumnName("created_at");
        entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

        entity.HasIndex(e => e.CurrentState).HasDatabaseName("ix_booking_saga_states_current_state");
    }
}
