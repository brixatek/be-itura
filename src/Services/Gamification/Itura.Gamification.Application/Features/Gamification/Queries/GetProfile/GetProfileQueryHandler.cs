using Itura.Gamification.Application.DTOs;
using Itura.Gamification.Domain.Entities;
using Itura.Gamification.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Gamification.Application.Features.Gamification.Queries.GetProfile;

internal sealed class GetProfileQueryHandler(IUserGamificationProfileRepository repository)
    : IRequestHandler<GetProfileQuery, Result<GamificationProfileDto>>
{
    public async Task<Result<GamificationProfileDto>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        var profile = await repository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (profile is null)
            return Result.Failure<GamificationProfileDto>(Error.NotFound("GamificationProfile", request.UserId));
        return ToDto(profile);
    }

    internal static GamificationProfileDto ToDto(UserGamificationProfile p) => new(
        p.Id, p.UserId, p.TotalPoints, p.Level, p.CurrentStreak, p.LongestStreak, p.LastActivityDate, p.CreatedAt);
}
