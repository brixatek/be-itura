using Itura.SharedKernel.Results;
using Itura.User.Application.Common.Interfaces;
using Itura.User.Domain.Repositories;
using MediatR;

namespace Itura.User.Application.Features.Users.UploadAvatar;

internal sealed class UploadAvatarCommandHandler(
    IUserProfileRepository profileRepository,
    IFileStorageService fileStorage,
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

        var url = await fileStorage.UploadAsync(request.FileStream, request.FileName, request.ContentType, cancellationToken);

        profile.UpdateProfile(profile.FullName, url, profile.Bio, profile.DateOfBirth, profile.Gender, profile.Timezone);
        profileRepository.Update(profile);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(url);
    }
}
