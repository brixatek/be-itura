using Itura.Gamification.Application.Common.Interfaces;

namespace Itura.Gamification.Infrastructure.Persistence;

internal sealed class UnitOfWork(GamificationDbContext context) : IGamificationUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
