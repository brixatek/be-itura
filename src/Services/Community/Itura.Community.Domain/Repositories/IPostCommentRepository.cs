using Itura.Community.Domain.Entities;
using Itura.SharedKernel.Results;

namespace Itura.Community.Domain.Repositories;

public interface IPostCommentRepository
{
    Task<PostComment?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<PagedResult<PostComment>> GetByPostIdAsync(Guid postId, int page, int pageSize, CancellationToken ct = default);
    Task AddAsync(PostComment comment, CancellationToken ct = default);
    void Update(PostComment comment);
}
