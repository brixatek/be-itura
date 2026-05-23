using Itura.User.Domain.Entities;
using Itura.User.Domain.Repositories;
using Itura.User.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Itura.User.Infrastructure.Repositories;

internal sealed class UserProfileRepository(UserDbContext dbContext) : IUserProfileRepository
{
    public async Task<UserProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.UserProfiles
            .Include(p => p.WellnessGoals)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.UserProfiles.AnyAsync(p => p.Id == id, cancellationToken);

    public async Task AddAsync(UserProfile profile, CancellationToken cancellationToken = default) =>
        await dbContext.UserProfiles.AddAsync(profile, cancellationToken);

    public void Update(UserProfile profile) =>
        dbContext.UserProfiles.Update(profile);
}
