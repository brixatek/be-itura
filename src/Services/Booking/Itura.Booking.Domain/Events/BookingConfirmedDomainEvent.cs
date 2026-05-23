using Itura.SharedKernel.Domain;

namespace Itura.Booking.Domain.Events;

public sealed record BookingConfirmedDomainEvent(
    Guid BookingId,
    Guid CoachId,
    Guid ClientUserId,
    DateTime ScheduledAt,
    int DurationMinutes,
    decimal Price,
    string Currency) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
