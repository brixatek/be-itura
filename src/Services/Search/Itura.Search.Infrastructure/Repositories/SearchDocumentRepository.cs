using Itura.Search.Domain.Entities;
using Itura.Search.Domain.Repositories;
using Itura.Search.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Itura.Search.Infrastructure.Repositories;

internal sealed class SearchDocumentRepository(SearchDbContext context) : ISearchDocumentRepository
{
    public async Task AddAsync(SearchDocument document, CancellationToken ct = default) =>
        await context.Documents.AddAsync(document, ct);

    public async Task<SearchDocument?> GetByEntityAsync(string entityType, Guid entityId, CancellationToken ct = default) =>
        await context.Documents.FirstOrDefaultAsync(d => d.EntityType == entityType && d.EntityId == entityId, ct);

    public void Update(SearchDocument document) =>
        context.Documents.Update(document);

    public async Task<(List<SearchDocument> Items, int TotalCount)> SearchAsync(
        string query, string? entityType, int page, int pageSize, CancellationToken ct = default)
    {
        var q = context.Documents.Where(d => d.IsActive);
        if (!string.IsNullOrWhiteSpace(entityType))
            q = q.Where(d => d.EntityType == entityType);
        if (!string.IsNullOrWhiteSpace(query))
            q = q.Where(d => EF.Functions.ILike(d.Title, $"%{query}%") ||
                             (d.Description != null && EF.Functions.ILike(d.Description, $"%{query}%")));
        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(d => d.UpdatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return (items, total);
    }
}
