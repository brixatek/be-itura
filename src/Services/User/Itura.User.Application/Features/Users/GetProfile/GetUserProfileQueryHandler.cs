using Itura.SharedKernel.Results;
using Itura.User.Application.DTOs;
using Itura.User.Domain.Repositories;
using MediatR;

namespace Itura.User.Application.Features.Users.GetProfile;

internal sealed class GetUserProfileQueryHandler(
    IUserProfileRepository profileRepository) : IRequestHandler<GetUserProfileQuery, Result<UserProfileDto>>
{
    public async Task<Result<UserProfileDto>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var profile = await profileRepository.GetByIdAsync(request.AccountId, cancellationToken);

        if (profile is null)
            return Result.Failure<UserProfileDto>(Error.NotFound("UserProfile", request.AccountId));

        var dto = new UserProfileDto(
            profile.Id,
            profile.Email,
            profile.FullName,
            profile.Timezone,
            profile.AvatarUrl,
            profile.Bio,
            profile.DateOfBirth,
            profile.Gender?.ToString(),
            profile.OnboardingCompleted,
            profile.WellnessGoals.Select(g => g.Goal).ToList().AsReadOnly(),
            new UserPreferencesDto(
                profile.EmailNotifications,
                profile.PushNotifications,
                profile.WeeklyDigest,
                profile.Theme,
                profile.Language));

        return Result.Success(dto);
    }
}
