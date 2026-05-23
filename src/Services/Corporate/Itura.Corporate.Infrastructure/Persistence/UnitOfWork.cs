using Itura.Corporate.Application.Common.Interfaces;

namespace Itura.Corporate.Infrastructure.Persistence;

internal sealed class UnitOfWork(CorporateDbContext context) : ICorporateUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
