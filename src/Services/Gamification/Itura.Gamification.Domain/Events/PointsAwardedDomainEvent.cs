using Itura.SharedKernel.Domain;

namespace Itura.Gamification.Domain.Events;

public sealed record PointsAwardedDomainEvent(
    Guid UserId,
    int Points,
    int NewTotal,
    int NewLevel,
    string Reason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
