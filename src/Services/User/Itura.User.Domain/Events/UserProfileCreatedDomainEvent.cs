using Itura.SharedKernel.Domain;

namespace Itura.User.Domain.Events;

public sealed record UserProfileCreatedDomainEvent(
    Guid EventId,
    DateTime OccurredAt,
    Guid AccountId,
    string Email,
    string FullName) : IDomainEvent
{
    public UserProfileCreatedDomainEvent(Guid accountId, string email, string fullName)
        : this(Guid.NewGuid(), DateTime.UtcNow, accountId, email, fullName) { }
}
