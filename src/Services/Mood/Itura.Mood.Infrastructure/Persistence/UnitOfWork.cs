using Itura.Mood.Application.Common.Interfaces;

namespace Itura.Mood.Infrastructure.Persistence;

internal sealed class UnitOfWork(MoodDbContext context) : IMoodUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
