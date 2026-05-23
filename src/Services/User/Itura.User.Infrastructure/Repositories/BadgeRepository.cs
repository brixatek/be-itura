using Itura.User.Domain.Entities;
using Itura.User.Domain.Repositories;
using Itura.User.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Itura.User.Infrastructure.Repositories;

internal sealed class BadgeRepository(UserDbContext context) : IBadgeRepository
{
    public Task<List<BadgeDefinition>> GetAllDefinitionsAsync(CancellationToken ct = default) =>
        context.BadgeDefinitions.ToListAsync(ct);

    public Task<BadgeDefinition?> GetDefinitionByNameAsync(string name, CancellationToken ct = default) =>
        context.BadgeDefinitions.FirstOrDefaultAsync(b => b.Name == name, ct);

    public Task<bool> HasEarnedAsync(Guid userProfileId, Guid badgeDefinitionId, CancellationToken ct = default) =>
        context.BadgesEarned.AnyAsync(
            b => b.UserProfileId == userProfileId && b.BadgeDefinitionId == badgeDefinitionId, ct);

    public Task<List<BadgeEarned>> GetEarnedByUserAsync(Guid userProfileId, CancellationToken ct = default) =>
        context.BadgesEarned
            .Include(b => b.BadgeDefinition)
            .Where(b => b.UserProfileId == userProfileId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync(ct);

    public async Task AddEarnedAsync(BadgeEarned badge, CancellationToken ct = default) =>
        await context.BadgesEarned.AddAsync(badge, ct);
}
