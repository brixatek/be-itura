using Itura.SharedKernel.Results;
using Itura.User.Application.Common.Interfaces;
using MediatR;

namespace Itura.User.Application.Features.Gamification;

internal sealed class GetLeaderboardQueryHandler(ILeaderboardCache leaderboard)
    : IRequestHandler<GetLeaderboardQuery, Result<List<LeaderboardEntry>>>
{
    public async Task<Result<List<LeaderboardEntry>>> Handle(GetLeaderboardQuery request, CancellationToken cancellationToken)
    {
        var entries = await leaderboard.GetTopAsync(Math.Clamp(request.Count, 1, 100), cancellationToken);
        return Result.Success(entries);
    }
}
