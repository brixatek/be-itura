using Itura.AI.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Itura.AI.Infrastructure.Services;

internal sealed class RedisAIRateLimiter(
    IConnectionMultiplexer redis,
    ILogger<RedisAIRateLimiter> logger) : IAIRateLimiter
{
    private readonly IDatabase _db = redis.GetDatabase();

    public async Task<RateLimitStatus> CheckAsync(Guid userId, int dailyLimit, CancellationToken ct = default)
    {
        try
        {
            var today = DateTime.UtcNow.ToString("yyyyMMdd");
            var key = $"ratelimit:ai:{userId:N}:{today}";
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var batch = _db.CreateBatch();
            var addTask = batch.SortedSetAddAsync(key, now.ToString(), now);
            var countTask = batch.SortedSetLengthAsync(key);
            var expireTask = batch.KeyExpireAsync(key, TimeSpan.FromDays(1));
            batch.Execute();

            await Task.WhenAll(addTask, countTask, expireTask);
            var count = await countTask;

            var resetAt = DateTime.UtcNow.Date.AddDays(1);
            var remaining = Math.Max(0, dailyLimit - (int)count);

            return count <= dailyLimit
                ? new RateLimitStatus(true, remaining, resetAt)
                : new RateLimitStatus(false, 0, resetAt);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Rate limiter Redis error for user {UserId} — allowing request", userId);
            return new RateLimitStatus(true, 1, DateTime.UtcNow.Date.AddDays(1));
        }
    }
}
