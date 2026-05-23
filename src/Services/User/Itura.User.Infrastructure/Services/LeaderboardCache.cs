using Itura.User.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Itura.User.Infrastructure.Services;

internal sealed class LeaderboardCache(IConnectionMultiplexer redis, ILogger<LeaderboardCache> logger)
    : ILeaderboardCache
{
    private const string Key = "leaderboard:xp";
    private readonly IDatabase _db = redis.GetDatabase();

    public async Task UpdateScoreAsync(Guid userId, int totalXp, CancellationToken ct = default)
    {
        try
        {
            await _db.SortedSetAddAsync(Key, userId.ToString("N"), totalXp);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to update leaderboard for user {UserId}", userId);
        }
    }

    public async Task<List<LeaderboardEntry>> GetTopAsync(int count, CancellationToken ct = default)
    {
        try
        {
            var entries = await _db.SortedSetRangeByRankWithScoresAsync(Key, 0, count - 1, Order.Descending);
            return entries.Select((e, i) => new LeaderboardEntry(
                Guid.Parse(e.Element.ToString()),
                (int)e.Score,
                i + 1)).ToList();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to get leaderboard");
            return [];
        }
    }

    public async Task<long?> GetRankAsync(Guid userId, CancellationToken ct = default)
    {
        try
        {
            var rank = await _db.SortedSetRankAsync(Key, userId.ToString("N"), Order.Descending);
            return rank.HasValue ? rank.Value + 1 : null;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to get rank for user {UserId}", userId);
            return null;
        }
    }
}
