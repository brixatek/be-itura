using Itura.Journal.Domain.Entities;

namespace Itura.Journal.Domain.Repositories;

public interface IJournalTemplateRepository
{
    Task<List<JournalTemplate>> GetAllAsync(CancellationToken ct = default);
    Task<List<JournalTemplate>> GetByCategoryAsync(string category, CancellationToken ct = default);
    Task<JournalTemplate?> GetByIdAsync(Guid id, CancellationToken ct = default);
}
