using Itura.Notification.Domain.Entities;
using Itura.Notification.Domain.Repositories;
using Itura.Notification.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Itura.Notification.Infrastructure.Repositories;

internal sealed class NotificationRepository(NotificationDbContext context) : INotificationRepository
{
    public async Task<UserNotification?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await context.Notifications.FirstOrDefaultAsync(n => n.Id == id, ct);

    public async Task<(IEnumerable<UserNotification> Items, int TotalCount)> GetByUserIdAsync(
        Guid userId, int page, int pageSize, CancellationToken ct = default)
    {
        var query = context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt);

        var total = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

        return (items, total);
    }

    public async Task<IEnumerable<UserNotification>> GetUnreadByUserIdAsync(Guid userId, CancellationToken ct = default) =>
        await context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync(ct);

    public async Task AddAsync(UserNotification notification, CancellationToken ct = default) =>
        await context.Notifications.AddAsync(notification, ct);

    public void Update(UserNotification notification) =>
        context.Notifications.Update(notification);

    public void UpdateRange(IEnumerable<UserNotification> notifications) =>
        context.Notifications.UpdateRange(notifications);
}
