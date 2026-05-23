using Itura.SharedKernel.Domain;

namespace Itura.Booking.Domain.Events;

public sealed record BookingCancelledDomainEvent(
    Guid BookingId,
    Guid CoachId,
    Guid ClientUserId,
    string? Reason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
