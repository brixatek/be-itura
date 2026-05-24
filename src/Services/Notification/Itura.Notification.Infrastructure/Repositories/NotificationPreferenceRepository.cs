using Itura.Notification.Domain.Entities;
using Itura.Notification.Domain.Repositories;
using Itura.Notification.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Itura.Notification.Infrastructure.Repositories;

internal sealed class NotificationPreferenceRepository(NotificationDbContext context)
    : INotificationPreferenceRepository
{
    public async Task<NotificationPreference?> GetByUserIdAsync(Guid userId, CancellationToken ct = default) =>
        await context.NotificationPreferences.FirstOrDefaultAsync(p => p.UserId == userId, ct);

    public async Task AddAsync(NotificationPreference preference, CancellationToken ct = default) =>
        await context.NotificationPreferences.AddAsync(preference, ct);

    public void Update(NotificationPreference preference) =>
        context.NotificationPreferences.Update(preference);
}
