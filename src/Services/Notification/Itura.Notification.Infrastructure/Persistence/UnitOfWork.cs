using Itura.Notification.Application.Common.Interfaces;

namespace Itura.Notification.Infrastructure.Persistence;

internal sealed class UnitOfWork(NotificationDbContext context) : INotificationUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
