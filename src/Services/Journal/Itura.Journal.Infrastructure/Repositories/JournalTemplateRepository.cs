using Itura.Journal.Domain.Entities;
using Itura.Journal.Domain.Repositories;
using Itura.Journal.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Itura.Journal.Infrastructure.Repositories;

internal sealed class JournalTemplateRepository(JournalDbContext context) : IJournalTemplateRepository
{
    public Task<List<JournalTemplate>> GetAllAsync(CancellationToken ct = default) =>
        context.JournalTemplates.OrderBy(t => t.Category).ThenBy(t => t.Name).ToListAsync(ct);

    public Task<List<JournalTemplate>> GetByCategoryAsync(string category, CancellationToken ct = default) =>
        context.JournalTemplates
            .Where(t => t.Category == category)
            .OrderBy(t => t.Name)
            .ToListAsync(ct);

    public Task<JournalTemplate?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        context.JournalTemplates.FirstOrDefaultAsync(t => t.Id == id, ct);
}
