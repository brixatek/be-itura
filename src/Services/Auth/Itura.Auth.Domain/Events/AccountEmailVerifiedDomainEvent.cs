using Itura.SharedKernel.Domain;

namespace Itura.Auth.Domain.Events;

public sealed record AccountEmailVerifiedDomainEvent(
    Guid EventId,
    DateTime OccurredAt,
    Guid AccountId,
    string Email) : IDomainEvent
{
    public AccountEmailVerifiedDomainEvent(Guid accountId, string email)
        : this(Guid.NewGuid(), DateTime.UtcNow, accountId, email) { }
}
