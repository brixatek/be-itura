using Itura.Coach.Domain.Entities;

namespace Itura.Coach.Domain.Repositories;

public interface IAvailabilityRepository
{
    Task<List<CoachAvailability>> GetByCoachUserIdAsync(Guid coachUserId, CancellationToken ct = default);
    Task<CoachAvailability?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(CoachAvailability availability, CancellationToken ct = default);
    void Remove(CoachAvailability availability);

    Task<List<CoachBlockedTime>> GetBlockedTimesAsync(Guid coachUserId, DateTime from, DateTime to, CancellationToken ct = default);
    Task AddBlockedTimeAsync(CoachBlockedTime blockedTime, CancellationToken ct = default);
}
