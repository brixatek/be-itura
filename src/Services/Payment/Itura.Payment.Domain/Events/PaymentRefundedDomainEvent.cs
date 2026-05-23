using Itura.SharedKernel.Domain;

namespace Itura.Payment.Domain.Events;

public sealed record PaymentRefundedDomainEvent(
    Guid PaymentId,
    Guid BookingId,
    Guid PayerUserId,
    Guid PayeeUserId,
    decimal Amount,
    string Currency) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
