using Itura.Content.Domain.Entities;
using Itura.Content.Domain.Enums;
using Itura.Content.Domain.Repositories;
using Itura.Content.Infrastructure.Persistence;
using Itura.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace Itura.Content.Infrastructure.Repositories;

internal sealed class ContentRepository(ContentDbContext context) : IContentRepository
{
    public async Task<ContentItem?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await context.ContentItems
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<ContentItem?> GetBySlugAsync(string slug, CancellationToken ct = default) =>
        await context.ContentItems
            .FirstOrDefaultAsync(e => e.Slug == slug, ct);

    public async Task<PagedResult<ContentItem>> GetAsync(
        int page,
        int pageSize,
        ContentType? contentType,
        string? tag,
        Guid? authorUserId,
        bool? isPublished,
        CancellationToken ct = default)
    {
        var query = context.ContentItems.AsQueryable();

        if (contentType.HasValue)
            query = query.Where(e => e.ContentType == contentType.Value);

        if (!string.IsNullOrWhiteSpace(tag))
            query = query.Where(e => e.Tags.Contains(tag));

        if (authorUserId.HasValue)
            query = query.Where(e => e.AuthorUserId == authorUserId.Value);

        if (isPublished.HasValue)
            query = query.Where(e => e.IsPublished == isPublished.Value);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(e => e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<ContentItem>(items, totalCount, page, pageSize);
    }

    public async Task AddAsync(ContentItem item, CancellationToken ct = default) =>
        await context.ContentItems.AddAsync(item, ct);

    public void Update(ContentItem item) =>
        context.ContentItems.Update(item);
}
