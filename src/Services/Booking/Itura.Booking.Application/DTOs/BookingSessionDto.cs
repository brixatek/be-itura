namespace Itura.Booking.Application.DTOs;

public sealed record BookingSessionDto(
    Guid Id,
    Guid CoachId,
    Guid CoachUserId,
    Guid ClientUserId,
    DateTime ScheduledAt,
    int DurationMinutes,
    string Status,
    decimal Price,
    string Currency,
    string? CancellationReason,
    DateTime CreatedAt,
    DateTime UpdatedAt);
