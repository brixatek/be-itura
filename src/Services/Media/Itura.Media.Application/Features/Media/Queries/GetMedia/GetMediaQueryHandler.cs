using Itura.Media.Application.DTOs;
using Itura.Media.Domain.Entities;
using Itura.Media.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Media.Application.Features.Media.Queries.GetMedia;

internal sealed class GetMediaQueryHandler(IMediaAssetRepository repository)
    : IRequestHandler<GetMediaQuery, Result<MediaAssetDto>>
{
    public async Task<Result<MediaAssetDto>> Handle(GetMediaQuery request, CancellationToken cancellationToken)
    {
        var asset = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (asset is null)
            return Result.Failure<MediaAssetDto>(Error.NotFound("MediaAsset", request.Id));

        if (!asset.IsPublic && asset.UploaderUserId != request.RequestingUserId)
            return Result.Failure<MediaAssetDto>(Error.Forbidden("This asset is private."));

        return ToDto(asset);
    }

    internal static MediaAssetDto ToDto(MediaAsset asset) => new(
        asset.Id,
        asset.FileName,
        asset.StoredFileName,
        asset.MimeType,
        asset.MediaType.ToString(),
        asset.FileSize,
        asset.Url,
        asset.UploaderUserId,
        asset.IsPublic,
        asset.CreatedAt);
}
