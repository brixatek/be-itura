using Itura.SharedKernel.Domain;

namespace Itura.User.Domain.Events;

public sealed record OnboardingCompletedDomainEvent(
    Guid EventId,
    DateTime OccurredAt,
    Guid AccountId) : IDomainEvent
{
    public OnboardingCompletedDomainEvent(Guid accountId)
        : this(Guid.NewGuid(), DateTime.UtcNow, accountId) { }
}
