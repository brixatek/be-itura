using Itura.Coach.Application.Common.Interfaces;

namespace Itura.Coach.Infrastructure.Persistence;

internal sealed class UnitOfWork(CoachDbContext context) : ICoachUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
