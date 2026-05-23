using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Booking.Application.Features.Bookings.CompleteBooking;

public sealed record CompleteBookingCommand(Guid BookingId, Guid UserId) : IRequest<Result>;
