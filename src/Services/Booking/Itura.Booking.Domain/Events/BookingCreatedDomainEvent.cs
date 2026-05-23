using Itura.SharedKernel.Domain;

namespace Itura.Booking.Domain.Events;

public sealed record BookingCreatedDomainEvent(
    Guid BookingId,
    Guid CoachId,
    Guid CoachUserId,
    Guid ClientUserId,
    DateTime ScheduledAt,
    int DurationMinutes,
    decimal Price,
    string Currency) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
