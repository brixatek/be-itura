using Itura.Journal.Application.Common.Interfaces;

namespace Itura.Journal.Infrastructure.Persistence;

internal sealed class UnitOfWork(JournalDbContext context) : IJournalUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
