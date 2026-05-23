using Itura.User.Domain.Entities;

namespace Itura.User.Domain.Repositories;

public interface IBadgeRepository
{
    Task<List<BadgeDefinition>> GetAllDefinitionsAsync(CancellationToken ct = default);
    Task<BadgeDefinition?> GetDefinitionByNameAsync(string name, CancellationToken ct = default);
    Task<bool> HasEarnedAsync(Guid userProfileId, Guid badgeDefinitionId, CancellationToken ct = default);
    Task<List<BadgeEarned>> GetEarnedByUserAsync(Guid userProfileId, CancellationToken ct = default);
    Task AddEarnedAsync(BadgeEarned badge, CancellationToken ct = default);
}
