using Itura.Gamification.Application.DTOs;
using Itura.Gamification.Application.Features.Gamification.Queries.GetProfile;
using Itura.Gamification.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Gamification.Application.Features.Gamification.Queries.GetLeaderboard;

internal sealed class GetLeaderboardQueryHandler(IUserGamificationProfileRepository repository)
    : IRequestHandler<GetLeaderboardQuery, Result<PagedResult<GamificationProfileDto>>>
{
    public async Task<Result<PagedResult<GamificationProfileDto>>> Handle(GetLeaderboardQuery request, CancellationToken cancellationToken)
    {
        var top = Math.Min(Math.Max(request.Top, 1), 100);
        var paged = await repository.GetLeaderboardAsync(top, cancellationToken);
        var dtos = paged.Items.Select(GetProfileQueryHandler.ToDto).ToList();
        return new PagedResult<GamificationProfileDto>(dtos, paged.TotalCount, 1, top);
    }
}
