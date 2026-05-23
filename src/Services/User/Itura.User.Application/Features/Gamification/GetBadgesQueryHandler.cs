using Itura.SharedKernel.Results;
using Itura.User.Domain.Repositories;
using MediatR;

namespace Itura.User.Application.Features.Gamification;

internal sealed class GetBadgesQueryHandler(IBadgeRepository badgeRepository)
    : IRequestHandler<GetBadgesQuery, Result<List<BadgeDto>>>
{
    public async Task<Result<List<BadgeDto>>> Handle(GetBadgesQuery request, CancellationToken cancellationToken)
    {
        var earned = await badgeRepository.GetEarnedByUserAsync(request.UserId, cancellationToken);
        var dtos = earned.Select(e => new BadgeDto(
            e.BadgeDefinitionId,
            e.BadgeDefinition?.Name ?? string.Empty,
            e.BadgeDefinition?.Description ?? string.Empty,
            e.BadgeDefinition?.IconUrl ?? string.Empty,
            e.CreatedAt)).ToList();

        return Result.Success(dtos);
    }
}
