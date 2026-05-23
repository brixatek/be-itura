using Itura.Booking.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Booking.Application.Features.Bookings.GetMyBookings;

public sealed record GetMyBookingsQuery(
    Guid ClientUserId,
    int Page,
    int PageSize) : IRequest<Result<PagedResult<BookingSessionDto>>>;
