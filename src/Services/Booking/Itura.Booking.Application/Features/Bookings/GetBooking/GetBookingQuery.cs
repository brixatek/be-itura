using Itura.Booking.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Booking.Application.Features.Bookings.GetBooking;

public sealed record GetBookingQuery(Guid BookingId, Guid UserId) : IRequest<Result<BookingSessionDto>>;
