using Itura.User.Domain.Entities;
using Itura.User.Domain.Repositories;
using Itura.User.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Itura.User.Infrastructure.Repositories;

internal sealed class StreakRepository(UserDbContext context) : IStreakRepository
{
    public Task<UserStreak?> GetAsync(Guid userProfileId, string streakType, CancellationToken ct = default) =>
        context.UserStreaks.FirstOrDefaultAsync(
            s => s.UserProfileId == userProfileId && s.StreakType == streakType, ct);

    public Task<List<UserStreak>> GetAtRiskAsync(DateOnly today, CancellationToken ct = default)
    {
        var yesterday = today.AddDays(-1);
        return context.UserStreaks
            .Where(s => s.LastActivityDate == yesterday && s.CurrentStreak > 0)
            .ToListAsync(ct);
    }

    public async Task AddAsync(UserStreak streak, CancellationToken ct = default) =>
        await context.UserStreaks.AddAsync(streak, ct);

    public void Update(UserStreak streak) =>
        context.UserStreaks.Update(streak);
}
