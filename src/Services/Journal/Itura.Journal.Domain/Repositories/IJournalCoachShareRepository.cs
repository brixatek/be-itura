using Itura.Journal.Domain.Entities;

namespace Itura.Journal.Domain.Repositories;

public interface IJournalCoachShareRepository
{
    Task<JournalCoachShare?> GetAsync(Guid journalEntryId, Guid coachId, CancellationToken ct = default);
    Task<List<JournalEntry>> GetSharedWithCoachAsync(Guid coachId, int page, int pageSize, CancellationToken ct = default);
    Task AddAsync(JournalCoachShare share, CancellationToken ct = default);
    void Update(JournalCoachShare share);
}
