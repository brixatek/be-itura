using Itura.Mood.Domain.Entities;

namespace Itura.Mood.Domain.Repositories;

public interface IMoodEntryRepository
{
    Task<MoodEntry?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<(IEnumerable<MoodEntry> Items, int TotalCount)> GetByUserIdAsync(
        Guid userId, int page, int pageSize,
        DateTime? from, DateTime? to,
        CancellationToken ct = default);
    Task<IEnumerable<MoodEntry>> GetByUserIdForPeriodAsync(
        Guid userId, DateTime from, DateTime to, CancellationToken ct = default);
    Task AddAsync(MoodEntry entry, CancellationToken ct = default);
    void Update(MoodEntry entry);
}
