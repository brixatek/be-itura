using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Booking.Application.Features.Bookings.RescheduleBooking;

public sealed record RescheduleBookingCommand(
    Guid BookingId,
    Guid RequestedByUserId,
    DateTime NewScheduledAt) : IRequest<Result>;
