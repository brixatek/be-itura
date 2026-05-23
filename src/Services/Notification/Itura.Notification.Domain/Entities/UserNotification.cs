using Itura.Notification.Domain.Enums;
using Itura.SharedKernel.Domain;

namespace Itura.Notification.Domain.Entities;

public sealed class UserNotification : AggregateRoot
{
    public Guid UserId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Body { get; private set; } = string.Empty;
    public NotificationType Type { get; private set; }
    public NotificationChannel Channel { get; private set; }
    public bool IsRead { get; private set; }
    public DateTime? ReadAt { get; private set; }

    private UserNotification() { }

    public static UserNotification Create(
        Guid userId, string title, string body,
        NotificationType type, NotificationChannel channel)
    {
        return new UserNotification
        {
            UserId = userId,
            Title = title,
            Body = body,
            Type = type,
            Channel = channel,
            IsRead = false
        };
    }

    public void MarkAsRead()
    {
        if (IsRead) return;
        IsRead = true;
        ReadAt = DateTime.UtcNow;
        MarkUpdated();
    }
}
