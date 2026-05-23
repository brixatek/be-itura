using Itura.Analytics.Domain.Entities;
using Itura.Analytics.Domain.Repositories;
using Itura.Analytics.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Itura.Analytics.Infrastructure.Repositories;

internal sealed class AnalyticsEventRepository(AnalyticsDbContext context) : IAnalyticsEventRepository
{
    public async Task AddAsync(AnalyticsEvent analyticsEvent, CancellationToken ct = default) =>
        await context.Events.AddAsync(analyticsEvent, ct);

    public async Task<(List<AnalyticsEvent> Items, int TotalCount)> GetUserEventsAsync(
        Guid userId, int page, int pageSize, CancellationToken ct = default)
    {
        var query = context.Events.Where(e => e.UserId == userId).OrderByDescending(e => e.OccurredAt);
        var total = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return (items, total);
    }

    public async Task<List<(string EventType, int Count)>> GetEventCountsAsync(
        DateTime from, DateTime to, CancellationToken ct = default)
    {
        return await context.Events
            .Where(e => e.OccurredAt >= from && e.OccurredAt <= to)
            .GroupBy(e => e.EventType)
            .Select(g => new { g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync(ct)
            .ContinueWith(t => t.Result.Select(x => (x.Key, x.Count)).ToList(), ct);
    }
}
