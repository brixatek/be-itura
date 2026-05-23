namespace Itura.Contracts.Auth;

public sealed record PasswordResetRequestedEvent(
    Guid AccountId,
    string Email,
    string ResetToken,
    DateTime ExpiresAt);
