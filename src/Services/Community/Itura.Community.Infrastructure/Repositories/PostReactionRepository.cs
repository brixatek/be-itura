using Itura.Community.Domain.Entities;
using Itura.Community.Domain.Repositories;
using Itura.Community.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Itura.Community.Infrastructure.Repositories;

internal sealed class PostReactionRepository(CommunityDbContext context) : IPostReactionRepository
{
    public Task<PostReaction?> GetAsync(Guid postId, Guid userId, string emoji, CancellationToken ct = default) =>
        context.PostReactions.FirstOrDefaultAsync(
            r => r.PostId == postId && r.UserId == userId && r.Emoji == emoji, ct);

    public Task<PostReaction?> GetByUserAndPostAsync(Guid postId, Guid userId, CancellationToken ct = default) =>
        context.PostReactions.FirstOrDefaultAsync(
            r => r.PostId == postId && r.UserId == userId, ct);

    public async Task<Dictionary<string, int>> GetCountsByPostIdAsync(Guid postId, CancellationToken ct = default) =>
        await context.PostReactions
            .Where(r => r.PostId == postId)
            .GroupBy(r => r.Emoji)
            .Select(g => new { Emoji = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Emoji, x => x.Count, ct);

    public async Task AddAsync(PostReaction reaction, CancellationToken ct = default) =>
        await context.PostReactions.AddAsync(reaction, ct);

    public void Remove(PostReaction reaction) =>
        context.PostReactions.Remove(reaction);
}
