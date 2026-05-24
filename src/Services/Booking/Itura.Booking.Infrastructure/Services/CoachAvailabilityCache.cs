using Itura.Booking.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace Itura.Booking.Infrastructure.Services;

internal sealed class CoachAvailabilityCache(
    IConnectionMultiplexer redis,
    ILogger<CoachAvailabilityCache> logger) : ICoachAvailabilityCache
{
    private readonly IDatabase _db = redis.GetDatabase();
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(5);

    private static string Key(Guid coachUserId, DateOnly date) =>
        $"coach_slots:{coachUserId:N}:{date:yyyyMMdd}";

    public async Task<List<DateTime>?> GetBookedSlotsAsync(Guid coachUserId, DateOnly date, CancellationToken ct = default)
    {
        try
        {
            var raw = await _db.StringGetAsync(Key(coachUserId, date));
            return raw.IsNull ? null : JsonSerializer.Deserialize<List<DateTime>>(raw!);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to read booked slots cache for coach {CoachUserId} on {Date}", coachUserId, date);
            return null;
        }
    }

    public async Task SetBookedSlotsAsync(Guid coachUserId, DateOnly date, List<DateTime> slots, CancellationToken ct = default)
    {
        try
        {
            var json = JsonSerializer.Serialize(slots);
            await _db.StringSetAsync(Key(coachUserId, date), json, CacheTtl);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to write booked slots cache for coach {CoachUserId} on {Date}", coachUserId, date);
        }
    }

    public async Task InvalidateAsync(Guid coachUserId, DateOnly date, CancellationToken ct = default)
    {
        try
        {
            await _db.KeyDeleteAsync(Key(coachUserId, date));
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to invalidate booked slots cache for coach {CoachUserId} on {Date}", coachUserId, date);
        }
    }
}
