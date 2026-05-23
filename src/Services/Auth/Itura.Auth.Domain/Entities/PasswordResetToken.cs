using Itura.SharedKernel.Entities;

namespace Itura.Auth.Domain.Entities;

public sealed class PasswordResetToken : BaseEntity
{
    private PasswordResetToken() { }

    public Guid AccountId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public DateTime? UsedAt { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public bool IsExpired => ExpiresAt < DateTime.UtcNow;
    public bool IsUsed => UsedAt.HasValue;
    public bool IsValid => !IsExpired && !IsUsed;

    public static PasswordResetToken Create(Guid accountId, string tokenHash, int expiryMinutes = 60)
    {
        return new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            TokenHash = tokenHash,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes),
        };
    }

    public void MarkUsed() => UsedAt = DateTime.UtcNow;
}
