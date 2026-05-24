using Itura.SharedKernel.Entities;

namespace Itura.Notification.Domain.Entities;

public sealed class NotificationPreference : AuditableEntity
{
    public Guid UserId { get; private set; }
    public bool PushEnabled { get; private set; } = true;
    public bool EmailEnabled { get; private set; } = true;
    public TimeOnly? QuietHoursStart { get; private set; }
    public TimeOnly? QuietHoursEnd { get; private set; }

    private NotificationPreference() { }

    public static NotificationPreference Create(Guid userId) =>
        new() { Id = Guid.NewGuid(), UserId = userId };

    public void Update(bool pushEnabled, bool emailEnabled, TimeOnly? quietStart, TimeOnly? quietEnd)
    {
        PushEnabled = pushEnabled;
        EmailEnabled = emailEnabled;
        QuietHoursStart = quietStart;
        QuietHoursEnd = quietEnd;
        MarkUpdated();
    }

    public bool IsQuietHoursNow()
    {
        if (QuietHoursStart is null || QuietHoursEnd is null) return false;
        var now = TimeOnly.FromDateTime(DateTime.UtcNow);
        if (QuietHoursStart < QuietHoursEnd)
            return now >= QuietHoursStart && now < QuietHoursEnd;
        // wraps midnight e.g. 22:00 → 07:00
        return now >= QuietHoursStart || now < QuietHoursEnd;
    }
}
