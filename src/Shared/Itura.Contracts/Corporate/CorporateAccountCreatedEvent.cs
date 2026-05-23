namespace Itura.Contracts.Corporate;

public sealed record CorporateAccountCreatedEvent(
    Guid CorporateAccountId,
    string CompanyName,
    Guid AdminUserId,
    string SubscriptionTier,
    DateTime CreatedAt);
