using Itura.User.Application.Common.Interfaces;

namespace Itura.User.Infrastructure.Persistence;

internal sealed class UnitOfWork(UserDbContext dbContext) : IUserUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
