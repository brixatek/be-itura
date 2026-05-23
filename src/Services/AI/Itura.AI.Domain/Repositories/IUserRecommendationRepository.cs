using Itura.AI.Domain.Entities;

namespace Itura.AI.Domain.Repositories;

public interface IUserRecommendationRepository
{
    Task AddAsync(UserRecommendation recommendation, CancellationToken ct = default);
    Task<List<UserRecommendation>> GetActiveByUserAsync(Guid userId, string? recommendationType, int limit, CancellationToken ct = default);
    Task DeactivateUserRecommendationsAsync(Guid userId, string recommendationType, CancellationToken ct = default);
}
