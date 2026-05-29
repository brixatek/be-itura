using Itura.Coach.Application.Common.Interfaces;
using Itura.Coach.Application.DTOs;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Itura.Coach.Infrastructure.Services;

internal sealed class SlotCache(IDistributedCache cache) : ISlotCache
{
    private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(10);

    public async Task<List<TimeSlotDto>?> GetAsync(
        Guid coachUserId, DateOnly date, string viewerTimezone, CancellationToken ct = default)
    {
        var key = BuildKey(coachUserId, date, viewerTimezone);
        var json = await cache.GetStringAsync(key, ct);
        if (json is null) return null;
        return JsonSerializer.Deserialize<List<TimeSlotDto>>(json);
    }

    public async Task SetAsync(
        Guid coachUserId, DateOnly date, string viewerTimezone, List<TimeSlotDto> slots, CancellationToken ct = default)
    {
        var key = BuildKey(coachUserId, date, viewerTimezone);
        var json = JsonSerializer.Serialize(slots);
        await cache.SetStringAsync(key, json,
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = Ttl }, ct);
    }

    public async Task InvalidateAsync(Guid coachUserId, CancellationToken ct = default)
    {
        // Pattern invalidation is not supported by IDistributedCache directly.
        // For Redis, a pattern-based delete would require IConnectionMultiplexer.
        // Here we store a "bust" key that forces cache misses for this coach.
        var bustKey = $"slots:bust:{coachUserId}";
        await cache.SetStringAsync(bustKey, DateTime.UtcNow.Ticks.ToString(),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1) }, ct);
    }

    private static string BuildKey(Guid coachUserId, DateOnly date, string viewerTimezone) =>
        $"slots:{coachUserId}:{date:yyyy-MM-dd}:{viewerTimezone}";
}
