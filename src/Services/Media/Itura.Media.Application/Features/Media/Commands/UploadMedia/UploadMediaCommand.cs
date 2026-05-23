using Itura.Media.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Media.Application.Features.Media.Commands.UploadMedia;

public sealed record UploadMediaCommand(
    string FileName,
    string MimeType,
    long FileSize,
    string MediaType,
    bool IsPublic,
    Guid UploaderUserId,
    byte[] FileContent) : IRequest<Result<MediaAssetDto>>;
