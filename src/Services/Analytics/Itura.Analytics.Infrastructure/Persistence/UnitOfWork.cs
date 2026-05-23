using Itura.Analytics.Application.Common.Interfaces;

namespace Itura.Analytics.Infrastructure.Persistence;

internal sealed class UnitOfWork(AnalyticsDbContext context) : IAnalyticsUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
