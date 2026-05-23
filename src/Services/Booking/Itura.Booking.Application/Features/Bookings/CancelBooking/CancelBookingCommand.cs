using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Booking.Application.Features.Bookings.CancelBooking;

public sealed record CancelBookingCommand(
    Guid BookingId,
    Guid UserId,
    string? Reason) : IRequest<Result>;
