using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Booking.Application.Features.Bookings.CreateBooking;

public sealed record CreateBookingCommand(
    Guid ClientUserId,
    Guid CoachId,
    Guid CoachUserId,
    DateTime ScheduledAt,
    int DurationMinutes,
    decimal Price,
    string Currency) : IRequest<Result<Guid>>;
