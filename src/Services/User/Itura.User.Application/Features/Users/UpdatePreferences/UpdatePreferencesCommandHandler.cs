using Itura.SharedKernel.Results;
using Itura.User.Application.Common.Interfaces;
using Itura.User.Domain.Repositories;
using MediatR;

namespace Itura.User.Application.Features.Users.UpdatePreferences;

internal sealed class UpdatePreferencesCommandHandler(
    IUserProfileRepository profileRepository,
    IUserUnitOfWork unitOfWork) : IRequestHandler<UpdatePreferencesCommand, Result>
{
    public async Task<Result> Handle(UpdatePreferencesCommand request, CancellationToken cancellationToken)
    {
        var profile = await profileRepository.GetByIdAsync(request.AccountId, cancellationToken);
        if (profile is null)
            return Result.Failure(Error.NotFound("UserProfile", request.AccountId));

        profile.UpdatePreferences(
            request.EmailNotifications,
            request.PushNotifications,
            request.WeeklyDigest,
            request.Theme,
            request.Language);

        profileRepository.Update(profile);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
