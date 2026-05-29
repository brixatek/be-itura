using Itura.SharedKernel.Domain;

namespace Itura.Auth.Domain.Events;

public sealed record AccountMarkedForDeletionDomainEvent(
    Guid AccountId,
    string Email,
    DateTime OccurredAt) : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
}
