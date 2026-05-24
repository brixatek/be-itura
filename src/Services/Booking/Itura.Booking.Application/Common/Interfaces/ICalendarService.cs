namespace Itura.Booking.Application.Common.Interfaces;

public interface ICalendarService
{
    string GenerateIcs(Guid bookingId, DateTime scheduledAt, int durationMinutes, string summary, string description);
}
