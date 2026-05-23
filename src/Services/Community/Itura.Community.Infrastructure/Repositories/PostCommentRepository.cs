using Itura.Community.Domain.Entities;
using Itura.Community.Domain.Repositories;
using Itura.Community.Infrastructure.Persistence;
using Itura.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace Itura.Community.Infrastructure.Repositories;

internal sealed class PostCommentRepository(CommunityDbContext context) : IPostCommentRepository
{
    public async Task<PostComment?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await context.Comments
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<PagedResult<PostComment>> GetByPostIdAsync(
        Guid postId, int page, int pageSize, CancellationToken ct = default)
    {
        var query = context.Comments.Where(e => e.PostId == postId);
        var total = await query.CountAsync(ct);
        var items = await query
            .OrderBy(e => e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<PostComment>(items, total, page, pageSize);
    }

    public async Task AddAsync(PostComment comment, CancellationToken ct = default) =>
        await context.Comments.AddAsync(comment, ct);

    public void Update(PostComment comment) =>
        context.Comments.Update(comment);
}
