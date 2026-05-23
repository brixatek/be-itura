using Itura.Gamification.Domain.Entities;
using Itura.Gamification.Domain.Repositories;
using Itura.Gamification.Infrastructure.Persistence;
using Itura.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace Itura.Gamification.Infrastructure.Repositories;

internal sealed class UserGamificationProfileRepository(GamificationDbContext context) : IUserGamificationProfileRepository
{
    public async Task<UserGamificationProfile?> GetByUserIdAsync(Guid userId, CancellationToken ct = default) =>
        await context.Profiles.FirstOrDefaultAsync(e => e.UserId == userId, ct);

    public async Task<PagedResult<UserGamificationProfile>> GetLeaderboardAsync(int top, CancellationToken ct = default)
    {
        var items = await context.Profiles
            .OrderByDescending(e => e.TotalPoints)
            .Take(top)
            .ToListAsync(ct);
        var total = await context.Profiles.CountAsync(ct);
        return new PagedResult<UserGamificationProfile>(items, total, 1, top);
    }

    public async Task AddAsync(UserGamificationProfile profile, CancellationToken ct = default) =>
        await context.Profiles.AddAsync(profile, ct);

    public void Update(UserGamificationProfile profile) => context.Profiles.Update(profile);
}
