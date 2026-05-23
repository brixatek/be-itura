using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Booking.Application.Features.Bookings.ConfirmBooking;

public sealed record ConfirmBookingCommand(Guid BookingId, Guid UserId) : IRequest<Result>;
