using Itura.SharedKernel.Domain;

namespace Itura.Booking.Domain.Events;

public sealed record BookingCompletedDomainEvent(
    Guid BookingId,
    Guid CoachId,
    Guid ClientUserId,
    decimal Price,
    string Currency) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
