using Itura.Community.Domain.Entities;
using Itura.Community.Domain.Enums;
using Itura.SharedKernel.Results;

namespace Itura.Community.Domain.Repositories;

public interface ICommunityPostRepository
{
    Task<CommunityPost?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<PagedResult<CommunityPost>> GetAsync(int page, int pageSize, PostType? postType, string? tag, Guid? authorUserId, CancellationToken ct = default);
    Task<List<CommunityPost>> GetFeedAsync(Guid? cursorId, DateTime? cursorCreatedAt, int limit, PostType? postType, string? tag, CancellationToken ct = default);
    Task<List<CommunityPost>> GetRecentAsync(DateTime since, CancellationToken ct = default);
    Task AddAsync(CommunityPost post, CancellationToken ct = default);
    void Update(CommunityPost post);
}
