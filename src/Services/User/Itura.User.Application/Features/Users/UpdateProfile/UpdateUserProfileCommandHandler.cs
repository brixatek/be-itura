using Itura.SharedKernel.Results;
using Itura.User.Application.Common.Interfaces;
using Itura.User.Domain.Enums;
using Itura.User.Domain.Repositories;
using MediatR;

namespace Itura.User.Application.Features.Users.UpdateProfile;

internal sealed class UpdateUserProfileCommandHandler(
    IUserProfileRepository profileRepository,
    IUserUnitOfWork unitOfWork) : IRequestHandler<UpdateUserProfileCommand, Result>
{
    public async Task<Result> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await profileRepository.GetByIdAsync(request.AccountId, cancellationToken);
        if (profile is null)
            return Result.Failure(Error.NotFound("UserProfile", request.AccountId));

        Gender? gender = request.Gender is not null && Enum.TryParse<Gender>(request.Gender, out var parsed)
            ? parsed
            : null;

        profile.UpdateProfile(request.FullName, request.AvatarUrl, request.Bio, request.DateOfBirth, gender, request.Timezone);
        profileRepository.Update(profile);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
