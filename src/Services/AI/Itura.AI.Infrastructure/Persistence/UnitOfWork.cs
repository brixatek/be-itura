using Itura.AI.Application.Common.Interfaces;

namespace Itura.AI.Infrastructure.Persistence;

internal sealed class UnitOfWork(AIDbContext context) : IAIUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
