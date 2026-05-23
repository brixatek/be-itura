using Itura.Coach.Domain.Entities;
using Itura.Coach.Domain.Repositories;
using Itura.Coach.Infrastructure.Persistence;
using Itura.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace Itura.Coach.Infrastructure.Repositories;

internal sealed class CoachRepository(CoachDbContext context) : ICoachRepository
{
    public async Task<CoachProfile?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await context.Coaches
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<CoachProfile?> GetByUserIdAsync(Guid userId, CancellationToken ct = default) =>
        await context.Coaches.FirstOrDefaultAsync(c => c.UserId == userId, ct);

    public async Task<PagedResult<CoachProfile>> GetActiveAsync(
        int page, int pageSize,
        string? specialization = null,
        string? language = null,
        double? minRating = null,
        decimal? maxHourlyRate = null,
        CancellationToken ct = default)
    {
        var query = context.Coaches.Where(c => c.IsActive);

        if (!string.IsNullOrWhiteSpace(specialization))
            query = query.Where(c => c.Specializations.Contains(specialization.Trim().ToLowerInvariant()));

        if (!string.IsNullOrWhiteSpace(language))
            query = query.Where(c => c.Languages.Contains(language.Trim().ToLowerInvariant()));

        if (minRating.HasValue)
            query = query.Where(c => c.AverageRating >= minRating.Value);

        if (maxHourlyRate.HasValue)
            query = query.Where(c => c.HourlyRate <= maxHourlyRate.Value);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(c => c.AverageRating)
            .ThenBy(c => c.HourlyRate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<CoachProfile>(items, total, page, pageSize);
    }

    public async Task AddAsync(CoachProfile coach, CancellationToken ct = default) =>
        await context.Coaches.AddAsync(coach, ct);

    public void Update(CoachProfile coach) =>
        context.Coaches.Update(coach);
}
