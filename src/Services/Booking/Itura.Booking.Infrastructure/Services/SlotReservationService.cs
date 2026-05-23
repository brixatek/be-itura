using Itura.Booking.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Itura.Booking.Infrastructure.Services;

internal sealed class SlotReservationService(
    IConnectionMultiplexer redis,
    ILogger<SlotReservationService> logger) : ISlotReservationService
{
    private readonly IDatabase _db = redis.GetDatabase();
    private static readonly TimeSpan ReservationTtl = TimeSpan.FromMinutes(10);

    private static string SlotKey(Guid coachUserId, DateTime scheduledAt) =>
        $"slot:{coachUserId:N}:{scheduledAt:yyyyMMddHHmm}";

    public async Task<bool> TryReserveAsync(Guid coachUserId, DateTime scheduledAt, Guid bookingId, CancellationToken ct = default)
    {
        try
        {
            var key = SlotKey(coachUserId, scheduledAt);
            return await _db.StringSetAsync(key, bookingId.ToString("N"), ReservationTtl, When.NotExists);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to check slot reservation for coach {CoachUserId} at {ScheduledAt}; allowing booking", coachUserId, scheduledAt);
            return true; // Fail open when Redis is unavailable
        }
    }

    public async Task ReleaseAsync(Guid coachUserId, DateTime scheduledAt, CancellationToken ct = default)
    {
        try
        {
            await _db.KeyDeleteAsync(SlotKey(coachUserId, scheduledAt));
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to release slot for coach {CoachUserId} at {ScheduledAt}", coachUserId, scheduledAt);
        }
    }
}
