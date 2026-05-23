using Itura.Media.Application.Common.Interfaces;
using Itura.Media.Application.DTOs;
using Itura.Media.Application.Features.Media.Queries.GetMedia;
using Itura.Media.Domain.Enums;
using Itura.Media.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Media.Application.Features.Media.Commands.UploadMedia;

internal sealed class UploadMediaCommandHandler(
    IMediaAssetRepository repository,
    IMediaUnitOfWork unitOfWork,
    IFileStorageService fileStorage)
    : IRequestHandler<UploadMediaCommand, Result<MediaAssetDto>>
{
    public async Task<Result<MediaAssetDto>> Handle(UploadMediaCommand request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<MediaType>(request.MediaType, true, out var mediaType))
            return Result.Failure<MediaAssetDto>(Error.Validation("MediaAsset.MediaType", "Invalid media type."));

        var storedFileName = await fileStorage.StoreAsync(
            request.FileContent, request.FileName, request.MimeType, cancellationToken);

        var url = fileStorage.GetUrl(storedFileName);

        var result = Domain.Entities.MediaAsset.Create(
            request.FileName,
            storedFileName,
            request.MimeType,
            mediaType,
            request.FileSize,
            url,
            request.UploaderUserId,
            request.IsPublic);

        if (result.IsFailure)
        {
            await fileStorage.DeleteAsync(storedFileName, cancellationToken);
            return Result.Failure<MediaAssetDto>(result.Error);
        }

        await repository.AddAsync(result.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return GetMediaQueryHandler.ToDto(result.Value);
    }
}
