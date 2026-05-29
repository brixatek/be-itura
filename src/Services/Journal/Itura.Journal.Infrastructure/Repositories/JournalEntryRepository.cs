using Itura.Journal.Domain.Entities;
using Itura.Journal.Domain.Repositories;
using Itura.Journal.Infrastructure.Persistence;
using Itura.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace Itura.Journal.Infrastructure.Repositories;

internal sealed class JournalEntryRepository(JournalDbContext context) : IJournalEntryRepository
{
    public async Task<JournalEntry?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await context.JournalEntries
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<PagedResult<JournalEntry>> GetByUserIdAsync(
        Guid userId, int page, int pageSize,
        string? tag = null, DateTime? from = null, DateTime? to = null,
        CancellationToken ct = default)
    {
        var query = context.JournalEntries
            .Where(e => e.UserId == userId);

        if (!string.IsNullOrWhiteSpace(tag))
            query = query.Where(e => e.Tags.Contains(tag.Trim().ToLowerInvariant()));

        if (from.HasValue)
            query = query.Where(e => e.CreatedAt >= from.Value);

        if (to.HasValue)
            query = query.Where(e => e.CreatedAt <= to.Value);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(e => e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<JournalEntry>(items, total, page, pageSize);
    }

    public async Task<IReadOnlyList<string>> GetTagsByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        var allTags = await context.JournalEntries
            .Where(e => e.UserId == userId)
            .Select(e => e.Tags)
            .ToListAsync(ct);

        return allTags.SelectMany(t => t).Distinct().OrderBy(t => t).ToList();
    }

    public async Task<int> CountByUserIdAsync(Guid userId, CancellationToken ct = default) =>
        await context.JournalEntries
            .Where(e => e.UserId == userId)
            .CountAsync(ct);

    public async Task<int> CountThisWeekAsync(Guid userId, CancellationToken ct = default)
    {
        var today = DateTime.UtcNow.Date;
        var daysSinceMonday = ((int)today.DayOfWeek + 6) % 7; // ISO week starts Monday
        var weekStart = today.AddDays(-daysSinceMonday);
        return await context.JournalEntries
            .Where(e => e.UserId == userId && e.CreatedAt >= weekStart)
            .CountAsync(ct);
    }

    public async Task AddAsync(JournalEntry entry, CancellationToken ct = default) =>
        await context.JournalEntries.AddAsync(entry, ct);

    public void Update(JournalEntry entry) =>
        context.JournalEntries.Update(entry);
}
