namespace Itura.Contracts.Auth;

public sealed record UserRegisteredEvent(
    Guid AccountId,
    string Email,
    string FullName,
    string Timezone,
    DateTime RegisteredAt);
