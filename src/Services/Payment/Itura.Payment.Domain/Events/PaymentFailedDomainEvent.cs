using Itura.SharedKernel.Domain;

namespace Itura.Payment.Domain.Events;

public sealed record PaymentFailedDomainEvent(
    Guid PaymentId,
    Guid BookingId,
    Guid PayerUserId,
    decimal Amount,
    string Currency,
    string? FailureReason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
