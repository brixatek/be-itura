using Itura.Community.Domain.Entities;
using Itura.Community.Domain.Enums;
using Itura.Community.Domain.Repositories;
using Itura.Community.Infrastructure.Persistence;
using Itura.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace Itura.Community.Infrastructure.Repositories;

internal sealed class CommunityPostRepository(CommunityDbContext context) : ICommunityPostRepository
{
    public async Task<CommunityPost?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await context.Posts
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<PagedResult<CommunityPost>> GetAsync(
        int page, int pageSize, PostType? postType, string? tag, Guid? authorUserId, CancellationToken ct = default)
    {
        var query = context.Posts.AsQueryable();

        if (postType.HasValue)
            query = query.Where(e => e.PostType == postType.Value);

        if (!string.IsNullOrWhiteSpace(tag))
            query = query.Where(e => e.Tags.Contains(tag));

        if (authorUserId.HasValue)
            query = query.Where(e => e.AuthorUserId == authorUserId.Value);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(e => e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<CommunityPost>(items, total, page, pageSize);
    }

    public async Task<List<CommunityPost>> GetFeedAsync(
        Guid? cursorId, DateTime? cursorCreatedAt, int limit,
        PostType? postType, string? tag, CancellationToken ct = default)
    {
        var query = context.Posts.AsQueryable();

        if (postType.HasValue)
            query = query.Where(e => e.PostType == postType.Value);

        if (!string.IsNullOrWhiteSpace(tag))
            query = query.Where(e => e.Tags.Contains(tag));

        if (cursorCreatedAt.HasValue && cursorId.HasValue)
            query = query.Where(e =>
                e.CreatedAt < cursorCreatedAt.Value ||
                (e.CreatedAt == cursorCreatedAt.Value && e.Id.CompareTo(cursorId.Value) < 0));

        return await query
            .OrderByDescending(e => e.CreatedAt)
            .ThenByDescending(e => e.Id)
            .Take(limit)
            .ToListAsync(ct);
    }

    public async Task<List<CommunityPost>> GetRecentAsync(DateTime since, CancellationToken ct = default) =>
        await context.Posts
            .Where(e => e.CreatedAt >= since)
            .ToListAsync(ct);

    public async Task AddAsync(CommunityPost post, CancellationToken ct = default) =>
        await context.Posts.AddAsync(post, ct);

    public void Update(CommunityPost post) =>
        context.Posts.Update(post);
}
