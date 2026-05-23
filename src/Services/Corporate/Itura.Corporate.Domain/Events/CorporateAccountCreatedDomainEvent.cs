using Itura.Corporate.Domain.Enums;
using Itura.SharedKernel.Domain;

namespace Itura.Corporate.Domain.Events;

public sealed record CorporateAccountCreatedDomainEvent(
    Guid CorporateAccountId,
    string CompanyName,
    Guid AdminUserId,
    SubscriptionTier SubscriptionTier) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
