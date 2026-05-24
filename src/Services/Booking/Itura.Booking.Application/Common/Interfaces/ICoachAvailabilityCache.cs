namespace Itura.Booking.Application.Common.Interfaces;

public interface ICoachAvailabilityCache
{
    Task<List<DateTime>?> GetBookedSlotsAsync(Guid coachUserId, DateOnly date, CancellationToken ct = default);
    Task SetBookedSlotsAsync(Guid coachUserId, DateOnly date, List<DateTime> slots, CancellationToken ct = default);
    Task InvalidateAsync(Guid coachUserId, DateOnly date, CancellationToken ct = default);
}
