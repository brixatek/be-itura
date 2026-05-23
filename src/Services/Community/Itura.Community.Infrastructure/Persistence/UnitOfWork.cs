using Itura.Community.Application.Common.Interfaces;

namespace Itura.Community.Infrastructure.Persistence;

internal sealed class UnitOfWork(CommunityDbContext context) : ICommunityUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
