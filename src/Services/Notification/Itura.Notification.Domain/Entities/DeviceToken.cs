using Itura.SharedKernel.Entities;

namespace Itura.Notification.Domain.Entities;

public sealed class DeviceToken : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Token { get; private set; } = string.Empty;
    public string Platform { get; private set; } = string.Empty; // ios, android, web
    public bool IsActive { get; private set; }
    public DateTime RegisteredAt { get; private set; }
    public DateTime? DeactivatedAt { get; private set; }

    private DeviceToken() { }

    public static DeviceToken Create(Guid userId, string token, string platform)
        => new()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = token,
            Platform = platform.ToLowerInvariant(),
            IsActive = true,
            RegisteredAt = DateTime.UtcNow
        };

    public void Deactivate()
    {
        IsActive = false;
        DeactivatedAt = DateTime.UtcNow;
    }
}
