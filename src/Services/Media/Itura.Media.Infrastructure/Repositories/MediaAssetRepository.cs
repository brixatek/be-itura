using Itura.Media.Domain.Entities;
using Itura.Media.Domain.Repositories;
using Itura.Media.Infrastructure.Persistence;
using Itura.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;

namespace Itura.Media.Infrastructure.Repositories;

internal sealed class MediaAssetRepository(MediaDbContext context) : IMediaAssetRepository
{
    public async Task<MediaAsset?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await context.MediaAssets
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<PagedResult<MediaAsset>> GetByUploaderUserIdAsync(
        Guid uploaderUserId, int page, int pageSize, CancellationToken ct = default)
    {
        var query = context.MediaAssets.Where(e => e.UploaderUserId == uploaderUserId);

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(e => e.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<MediaAsset>(items, totalCount, page, pageSize);
    }

    public async Task AddAsync(MediaAsset asset, CancellationToken ct = default) =>
        await context.MediaAssets.AddAsync(asset, ct);

    public void Update(MediaAsset asset) =>
        context.MediaAssets.Update(asset);
}
