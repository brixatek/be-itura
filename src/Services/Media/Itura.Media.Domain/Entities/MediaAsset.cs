using Itura.Media.Domain.Enums;
using Itura.Media.Domain.Events;
using Itura.SharedKernel.Domain;
using Itura.SharedKernel.Results;

namespace Itura.Media.Domain.Entities;

public sealed class MediaAsset : AggregateRoot
{
    public string FileName { get; private set; } = string.Empty;
    public string StoredFileName { get; private set; } = string.Empty;
    public string MimeType { get; private set; } = string.Empty;
    public MediaType MediaType { get; private set; }
    public long FileSize { get; private set; }
    public string Url { get; private set; } = string.Empty;
    public Guid UploaderUserId { get; private set; }
    public bool IsPublic { get; private set; }

    private MediaAsset() { }

    public static Result<MediaAsset> Create(
        string fileName,
        string storedFileName,
        string mimeType,
        MediaType mediaType,
        long fileSize,
        string url,
        Guid uploaderUserId,
        bool isPublic)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return Result.Failure<MediaAsset>(Error.Validation("MediaAsset.FileName", "File name is required."));

        if (uploaderUserId == Guid.Empty)
            return Result.Failure<MediaAsset>(Error.Validation("MediaAsset.UploaderUserId", "Uploader is required."));

        if (fileSize <= 0)
            return Result.Failure<MediaAsset>(Error.Validation("MediaAsset.FileSize", "File size must be greater than zero."));

        var asset = new MediaAsset
        {
            FileName = fileName,
            StoredFileName = storedFileName,
            MimeType = mimeType,
            MediaType = mediaType,
            FileSize = fileSize,
            Url = url,
            UploaderUserId = uploaderUserId,
            IsPublic = isPublic
        };

        asset.RaiseDomainEvent(new MediaAssetUploadedDomainEvent(
            asset.Id, asset.Url, asset.MediaType, asset.MimeType, asset.UploaderUserId));

        return asset;
    }

    public void Delete() => MarkDeleted();
}
