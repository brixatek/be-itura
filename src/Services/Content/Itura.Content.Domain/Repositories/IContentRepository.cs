using Itura.Content.Domain.Entities;
using Itura.Content.Domain.Enums;
using Itura.SharedKernel.Results;

namespace Itura.Content.Domain.Repositories;

public interface IContentRepository
{
    Task<ContentItem?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ContentItem?> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task<PagedResult<ContentItem>> GetAsync(int page, int pageSize, ContentType? contentType, string? tag, Guid? authorUserId, bool? isPublished, CancellationToken ct = default);
    Task AddAsync(ContentItem item, CancellationToken ct = default);
    void Update(ContentItem item);
}
