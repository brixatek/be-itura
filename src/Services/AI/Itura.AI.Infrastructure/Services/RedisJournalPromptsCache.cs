using Itura.AI.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace Itura.AI.Infrastructure.Services;

internal sealed class RedisJournalPromptsCache(
    IConnectionMultiplexer redis,
    ILogger<RedisJournalPromptsCache> logger) : IJournalPromptsCache
{
    private readonly IDatabase _db = redis.GetDatabase();

    private static string Key(Guid userId) =>
        $"journal:prompts:{userId:N}:{DateTime.UtcNow:yyyyMMdd}";

    public async Task<List<string>?> GetAsync(Guid userId, CancellationToken ct = default)
    {
        try
        {
            var json = await _db.StringGetAsync(Key(userId));
            return json.IsNull ? null : JsonSerializer.Deserialize<List<string>>(json!);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to get journal prompts from cache for {UserId}", userId);
            return null;
        }
    }

    public async Task SetAsync(Guid userId, List<string> prompts, CancellationToken ct = default)
    {
        try
        {
            var midnight = DateTime.UtcNow.Date.AddDays(1);
            var ttl = midnight - DateTime.UtcNow;
            await _db.StringSetAsync(Key(userId), JsonSerializer.Serialize(prompts), ttl);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to cache journal prompts for {UserId}", userId);
        }
    }
}
