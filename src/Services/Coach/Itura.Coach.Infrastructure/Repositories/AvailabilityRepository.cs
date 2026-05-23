using Itura.Coach.Domain.Entities;
using Itura.Coach.Domain.Repositories;
using Itura.Coach.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Itura.Coach.Infrastructure.Repositories;

internal sealed class AvailabilityRepository(CoachDbContext context) : IAvailabilityRepository
{
    public async Task<List<CoachAvailability>> GetByCoachUserIdAsync(Guid coachUserId, CancellationToken ct = default) =>
        await context.CoachAvailabilities.Where(a => a.CoachUserId == coachUserId).ToListAsync(ct);

    public async Task<CoachAvailability?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await context.CoachAvailabilities.FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task AddAsync(CoachAvailability availability, CancellationToken ct = default) =>
        await context.CoachAvailabilities.AddAsync(availability, ct);

    public void Remove(CoachAvailability availability) =>
        context.CoachAvailabilities.Remove(availability);

    public async Task<List<CoachBlockedTime>> GetBlockedTimesAsync(
        Guid coachUserId, DateTime from, DateTime to, CancellationToken ct = default) =>
        await context.CoachBlockedTimes
            .Where(b => b.CoachUserId == coachUserId && b.StartUtc < to && b.EndUtc > from)
            .ToListAsync(ct);

    public async Task AddBlockedTimeAsync(CoachBlockedTime blockedTime, CancellationToken ct = default) =>
        await context.CoachBlockedTimes.AddAsync(blockedTime, ct);
}
