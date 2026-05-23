using Itura.Mood.Domain.Entities;
using Itura.Mood.Domain.Repositories;
using Itura.Mood.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Itura.Mood.Infrastructure.Repositories;

internal sealed class MoodEntryRepository(MoodDbContext context) : IMoodEntryRepository
{
    public async Task<MoodEntry?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await context.MoodEntries.IgnoreQueryFilters()
            .FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<(IEnumerable<MoodEntry> Items, int TotalCount)> GetByUserIdAsync(
        Guid userId, int page, int pageSize,
        DateTime? from, DateTime? to,
        CancellationToken ct = default)
    {
        var query = context.MoodEntries
            .Where(e => e.UserId == userId);

        if (from.HasValue) query = query.Where(e => e.LoggedAt >= from.Value);
        if (to.HasValue) query = query.Where(e => e.LoggedAt <= to.Value);

        query = query.OrderByDescending(e => e.LoggedAt);

        var total = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

        return (items, total);
    }

    public async Task<IEnumerable<MoodEntry>> GetByUserIdForPeriodAsync(
        Guid userId, DateTime from, DateTime to, CancellationToken ct = default) =>
        await context.MoodEntries
            .Where(e => e.UserId == userId && e.LoggedAt >= from && e.LoggedAt <= to)
            .OrderBy(e => e.LoggedAt)
            .ToListAsync(ct);

    public async Task AddAsync(MoodEntry entry, CancellationToken ct = default) =>
        await context.MoodEntries.AddAsync(entry, ct);

    public void Update(MoodEntry entry) =>
        context.MoodEntries.Update(entry);
}
