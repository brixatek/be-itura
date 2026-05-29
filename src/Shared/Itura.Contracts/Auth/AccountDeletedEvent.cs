namespace Itura.Contracts.Auth;

public sealed record AccountDeletedEvent(
    Guid AccountId,
    string Email,
    DateTime DeletedAt);
