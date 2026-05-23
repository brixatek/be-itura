using Itura.Gamification.Domain.Entities;
using Itura.SharedKernel.Results;

namespace Itura.Gamification.Domain.Repositories;

public interface IUserGamificationProfileRepository
{
    Task<UserGamificationProfile?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<PagedResult<UserGamificationProfile>> GetLeaderboardAsync(int top, CancellationToken ct = default);
    Task AddAsync(UserGamificationProfile profile, CancellationToken ct = default);
    void Update(UserGamificationProfile profile);
}
