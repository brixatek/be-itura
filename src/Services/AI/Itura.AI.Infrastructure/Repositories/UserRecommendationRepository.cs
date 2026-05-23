using Itura.AI.Domain.Entities;
using Itura.AI.Domain.Repositories;
using Itura.AI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Itura.AI.Infrastructure.Repositories;

internal sealed class UserRecommendationRepository(AIDbContext context) : IUserRecommendationRepository
{
    public async Task AddAsync(UserRecommendation recommendation, CancellationToken ct = default) =>
        await context.Recommendations.AddAsync(recommendation, ct);

    public async Task<List<UserRecommendation>> GetActiveByUserAsync(
        Guid userId, string? recommendationType, int limit, CancellationToken ct = default)
    {
        var query = context.Recommendations
            .Where(r => r.UserId == userId && r.IsActive && r.ExpiresAt > DateTime.UtcNow);
        if (!string.IsNullOrWhiteSpace(recommendationType))
            query = query.Where(r => r.RecommendationType == recommendationType);
        return await query.OrderByDescending(r => r.Score).Take(limit).ToListAsync(ct);
    }

    public async Task DeactivateUserRecommendationsAsync(Guid userId, string recommendationType, CancellationToken ct = default)
    {
        await context.Recommendations
            .Where(r => r.UserId == userId && r.RecommendationType == recommendationType && r.IsActive)
            .ExecuteUpdateAsync(s => s.SetProperty(r => r.IsActive, false), ct);
    }
}
