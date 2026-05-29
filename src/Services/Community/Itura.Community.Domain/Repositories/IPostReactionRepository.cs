using Itura.Community.Domain.Entities;

namespace Itura.Community.Domain.Repositories;

public interface IPostReactionRepository
{
    Task<PostReaction?> GetAsync(Guid postId, Guid userId, string emoji, CancellationToken ct = default);
    Task<PostReaction?> GetByUserAndPostAsync(Guid postId, Guid userId, CancellationToken ct = default);
    Task<Dictionary<string, int>> GetCountsByPostIdAsync(Guid postId, CancellationToken ct = default);
    Task AddAsync(PostReaction reaction, CancellationToken ct = default);
    void Remove(PostReaction reaction);
}
