using Itura.Journal.Domain.Entities;
using Itura.SharedKernel.Results;

namespace Itura.Journal.Domain.Repositories;

public interface IJournalEntryRepository
{
    Task<JournalEntry?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<PagedResult<JournalEntry>> GetByUserIdAsync(
        Guid userId, int page, int pageSize,
        string? tag = null, DateTime? from = null, DateTime? to = null,
        CancellationToken ct = default);
    Task<IReadOnlyList<string>> GetTagsByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<int> CountByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<int> CountThisWeekAsync(Guid userId, CancellationToken ct = default);
    Task AddAsync(JournalEntry entry, CancellationToken ct = default);
    void Update(JournalEntry entry);
}
