using Itura.Search.Application.Common.Interfaces;

namespace Itura.Search.Infrastructure.Persistence;

internal sealed class UnitOfWork(SearchDbContext context) : ISearchUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
