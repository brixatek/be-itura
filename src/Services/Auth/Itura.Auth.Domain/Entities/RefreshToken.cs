using Itura.SharedKernel.Entities;

namespace Itura.Auth.Domain.Entities;

public sealed class RefreshToken : BaseEntity
{
    private RefreshToken() { }

    public Guid AccountId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public string Jti { get; private set; } = string.Empty;
    public string? DeviceInfo { get; private set; }
    public string? IpAddress { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public Guid? ReplacedBy { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public bool IsExpired => ExpiresAt < DateTime.UtcNow;
    public bool IsRevoked => RevokedAt.HasValue;
    public bool IsActive => !IsExpired && !IsRevoked;

    public static RefreshToken Create(Guid accountId, string tokenHash, string jti,
        string? deviceInfo, string? ipAddress, int expiryDays = 30)
    {
        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            TokenHash = tokenHash,
            Jti = jti,
            DeviceInfo = deviceInfo,
            IpAddress = ipAddress,
            ExpiresAt = DateTime.UtcNow.AddDays(expiryDays),
        };
    }

    public void Revoke(Guid? replacedBy = null)
    {
        RevokedAt = DateTime.UtcNow;
        ReplacedBy = replacedBy;
    }
}
