using Itura.SharedKernel.Results;
using Itura.User.Application.Common.Interfaces;
using Itura.User.Domain.Repositories;
using MediatR;

namespace Itura.User.Application.Features.Users.UploadAvatar;

internal sealed class UploadAvatarCommandHandler(
    IUserProfileRepository profileRepository,
    IFileStorageService fileStorage,
    IImageResizingService imageResizing,
    IUserUnitOfWork unitOfWork)
    : IRequestHandler<UploadAvatarCommand, Result<string>>
{
    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB
    private static readonly HashSet<string> AllowedTypes = ["image/jpeg", "image/png", "image/webp"];

    public async Task<Result<string>> Handle(UploadAvatarCommand request, CancellationToken cancellationToken)
    {
        if (request.SizeBytes > MaxFileSizeBytes)
            return Result.Failure<string>(Error.Validation("Avatar.TooLarge", "Avatar must be 5 MB or smaller."));

        if (!AllowedTypes.Contains(request.ContentType.ToLowerInvariant()))
            return Result.Failure<string>(Error.Validation("Avatar.InvalidType", "Avatar must be JPEG, PNG, or WebP."));

        var profile = await profileRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (profile is null)
            return Result.Failure<string>(Error.NotFound("UserProfile", request.UserId));

        // Read stream into memory once so we can resize multiple times
        using var buffer = new MemoryStream();
        await request.FileStream.CopyToAsync(buffer, cancellationToken);
        buffer.Position = 0;

        // Upload 400×400 (display size — this is the primary URL stored)
        buffer.Position = 0;
        await using var stream400 = await imageResizing.ResizeAsync(buffer, 400, 400, cancellationToken);
        var url400 = await fileStorage.UploadAsync(stream400, $"avatar_400_{request.FileName}", "image/jpeg", cancellationToken);

        // Upload 200×200 (thumbnail)
        buffer.Position = 0;
        await using var stream200 = await imageResizing.ResizeAsync(buffer, 200, 200, cancellationToken);
        await fileStorage.UploadAsync(stream200, $"avatar_200_{request.FileName}", "image/jpeg", cancellationToken);

        profile.UpdateProfile(profile.FullName, url400, profile.Bio, profile.DateOfBirth, profile.Gender, profile.Timezone);
        profileRepository.Update(profile);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(url400);
    }
}
