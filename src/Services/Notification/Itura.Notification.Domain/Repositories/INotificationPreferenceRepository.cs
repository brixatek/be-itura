using Itura.Notification.Domain.Entities;

namespace Itura.Notification.Domain.Repositories;

public interface INotificationPreferenceRepository
{
    Task<NotificationPreference?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task AddAsync(NotificationPreference preference, CancellationToken ct = default);
    void Update(NotificationPreference preference);
}
