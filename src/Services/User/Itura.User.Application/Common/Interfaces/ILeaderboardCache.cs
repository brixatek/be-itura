namespace Itura.User.Application.Common.Interfaces;

public interface ILeaderboardCache
{
    Task UpdateScoreAsync(Guid userId, int totalXp, CancellationToken ct = default);
    Task<List<LeaderboardEntry>> GetTopAsync(int count, CancellationToken ct = default);
    Task<long?> GetRankAsync(Guid userId, CancellationToken ct = default);
}

public sealed record LeaderboardEntry(Guid UserId, int TotalXp, long Rank);
