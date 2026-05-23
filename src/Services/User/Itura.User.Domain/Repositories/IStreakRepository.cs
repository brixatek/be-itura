using Itura.User.Domain.Entities;

namespace Itura.User.Domain.Repositories;

public interface IStreakRepository
{
    Task<UserStreak?> GetAsync(Guid userProfileId, string streakType, CancellationToken ct = default);
    Task<List<UserStreak>> GetAtRiskAsync(DateOnly today, CancellationToken ct = default);
    Task AddAsync(UserStreak streak, CancellationToken ct = default);
    void Update(UserStreak streak);
}
