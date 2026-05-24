using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.User.Application.Features.Users.UploadAvatar;

public sealed record UploadAvatarCommand(
    Guid UserId,
    Stream FileStream,
    string FileName,
    string ContentType,
    long SizeBytes) : IRequest<Result<string>>;
