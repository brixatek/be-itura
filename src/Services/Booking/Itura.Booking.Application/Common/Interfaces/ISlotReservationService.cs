namespace Itura.Booking.Application.Common.Interfaces;

public interface ISlotReservationService
{
    Task<bool> TryReserveAsync(Guid coachUserId, DateTime scheduledAt, Guid bookingId, CancellationToken ct = default);
    Task ReleaseAsync(Guid coachUserId, DateTime scheduledAt, CancellationToken ct = default);
}
