using Itura.Search.Domain.Entities;

namespace Itura.Search.Domain.Repositories;

public interface ISearchDocumentRepository
{
    Task AddAsync(SearchDocument document, CancellationToken ct = default);
    Task<SearchDocument?> GetByEntityAsync(string entityType, Guid entityId, CancellationToken ct = default);
    void Update(SearchDocument document);
    Task<(List<SearchDocument> Items, int TotalCount)> SearchAsync(string query, string? entityType, int page, int pageSize, CancellationToken ct = default);
}
