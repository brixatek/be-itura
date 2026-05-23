using Itura.Booking.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Booking.Application.Features.Bookings.GetCoachBookings;

public sealed record GetCoachBookingsQuery(
    Guid CoachUserId,
    int Page,
    int PageSize) : IRequest<Result<PagedResult<BookingSessionDto>>>;
