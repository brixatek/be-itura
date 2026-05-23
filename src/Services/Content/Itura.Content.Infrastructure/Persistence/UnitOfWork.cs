using Itura.Content.Application.Common.Interfaces;

namespace Itura.Content.Infrastructure.Persistence;

internal sealed class UnitOfWork(ContentDbContext context) : IContentUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
