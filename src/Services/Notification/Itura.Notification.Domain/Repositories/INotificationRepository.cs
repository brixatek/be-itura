using Itura.Notification.Domain.Entities;

namespace Itura.Notification.Domain.Repositories;

public interface INotificationRepository
{
    Task<UserNotification?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<(IEnumerable<UserNotification> Items, int TotalCount)> GetByUserIdAsync(
        Guid userId, int page, int pageSize, CancellationToken ct = default);
    Task<IEnumerable<UserNotification>> GetUnreadByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task AddAsync(UserNotification notification, CancellationToken ct = default);
    void Update(UserNotification notification);
    void UpdateRange(IEnumerable<UserNotification> notifications);
}
