using Itura.SharedKernel.Results;
using Itura.User.Application.Common.Interfaces;
using Itura.User.Application.DTOs;
using Itura.User.Domain.Repositories;
using MediatR;

namespace Itura.User.Application.Features.Users.GetPreferences;

internal sealed class GetPreferencesQueryHandler(
    IUserProfileRepository profileRepository,
    IPreferencesCache cache)
    : IRequestHandler<GetPreferencesQuery, Result<UserPreferencesDto>>
{
    public async Task<Result<UserPreferencesDto>> Handle(GetPreferencesQuery request, CancellationToken cancellationToken)
    {
        var cached = await cache.GetAsync(request.AccountId, cancellationToken);
        if (cached is not null)
            return Result.Success(cached);

        var profile = await profileRepository.GetByIdAsync(request.AccountId, cancellationToken);
        if (profile is null)
            return Result.Failure<UserPreferencesDto>(Error.NotFound("UserProfile", request.AccountId));

        var dto = new UserPreferencesDto(
            profile.EmailNotifications,
            profile.PushNotifications,
            profile.WeeklyDigest,
            profile.Theme,
            profile.Language);

        await cache.SetAsync(request.AccountId, dto, cancellationToken);
        return Result.Success(dto);
    }
}
