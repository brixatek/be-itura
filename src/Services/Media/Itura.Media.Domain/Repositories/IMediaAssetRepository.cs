using Itura.Media.Domain.Entities;
using Itura.SharedKernel.Results;

namespace Itura.Media.Domain.Repositories;

public interface IMediaAssetRepository
{
    Task<MediaAsset?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<PagedResult<MediaAsset>> GetByUploaderUserIdAsync(Guid uploaderUserId, int page, int pageSize, CancellationToken ct = default);
    Task AddAsync(MediaAsset asset, CancellationToken ct = default);
    void Update(MediaAsset asset);
}
