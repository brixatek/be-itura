namespace Itura.Contracts.Media;

public sealed record MediaAssetUploadedEvent(
    Guid MediaAssetId,
    string Url,
    string MediaType,
    string MimeType,
    Guid UploaderUserId,
    DateTime UploadedAt);
