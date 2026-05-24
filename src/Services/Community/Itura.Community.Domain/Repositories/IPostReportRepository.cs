using Itura.Community.Domain.Entities;

namespace Itura.Community.Domain.Repositories;

public interface IPostReportRepository
{
    Task<bool> ExistsAsync(Guid postId, Guid reporterUserId, CancellationToken ct = default);
    Task AddAsync(PostReport report, CancellationToken ct = default);
}
