using Itura.Media.Application.Common.Interfaces;

namespace Itura.Media.Infrastructure.Persistence;

internal sealed class UnitOfWork(MediaDbContext context) : IMediaUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
