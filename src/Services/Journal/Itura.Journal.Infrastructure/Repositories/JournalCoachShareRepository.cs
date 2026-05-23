using Itura.Journal.Domain.Entities;
using Itura.Journal.Domain.Repositories;
using Itura.Journal.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Itura.Journal.Infrastructure.Repositories;

internal sealed class JournalCoachShareRepository(JournalDbContext context) : IJournalCoachShareRepository
{
    public Task<JournalCoachShare?> GetAsync(Guid journalEntryId, Guid coachId, CancellationToken ct = default) =>
        context.JournalCoachShares
            .Where(s => s.JournalEntryId == journalEntryId && s.CoachId == coachId)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync(ct);

    public async Task<List<JournalEntry>> GetSharedWithCoachAsync(Guid coachId, int page, int pageSize, CancellationToken ct = default) =>
        await context.JournalCoachShares
            .Where(s => s.CoachId == coachId && s.RevokedAt == null)
            .OrderByDescending(s => s.SharedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Join(context.JournalEntries,
                s => s.JournalEntryId,
                e => e.Id,
                (s, e) => e)
            .ToListAsync(ct);

    public async Task AddAsync(JournalCoachShare share, CancellationToken ct = default) =>
        await context.JournalCoachShares.AddAsync(share, ct);

    public void Update(JournalCoachShare share) =>
        context.JournalCoachShares.Update(share);
}
