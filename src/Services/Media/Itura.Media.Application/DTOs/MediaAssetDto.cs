namespace Itura.Media.Application.DTOs;

public sealed record MediaAssetDto(
    Guid Id,
    string FileName,
    string StoredFileName,
    string MimeType,
    string MediaType,
    long FileSize,
    string Url,
    Guid UploaderUserId,
    bool IsPublic,
    DateTime CreatedAt);
