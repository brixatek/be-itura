using Itura.Auth.Application.Common.Interfaces;

namespace Itura.Auth.Infrastructure.Persistence;

internal sealed class UnitOfWork(AuthDbContext context) : IAuthUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
