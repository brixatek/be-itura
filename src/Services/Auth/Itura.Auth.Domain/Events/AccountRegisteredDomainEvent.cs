using Itura.SharedKernel.Domain;

namespace Itura.Auth.Domain.Events;

public sealed record AccountRegisteredDomainEvent(
    Guid EventId,
    DateTime OccurredAt,
    Guid AccountId,
    string Email,
    string FullName,
    string Timezone) : IDomainEvent
{
    public AccountRegisteredDomainEvent(Guid accountId, string email, string fullName, string timezone)
        : this(Guid.NewGuid(), DateTime.UtcNow, accountId, email, fullName, timezone) { }
}
