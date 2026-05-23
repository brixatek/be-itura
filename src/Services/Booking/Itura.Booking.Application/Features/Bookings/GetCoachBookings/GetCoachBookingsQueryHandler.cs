using Itura.Booking.Application.DTOs;
using Itura.Booking.Application.Features.Bookings.GetBooking;
using Itura.Booking.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Booking.Application.Features.Bookings.GetCoachBookings;

internal sealed class GetCoachBookingsQueryHandler(IBookingRepository repository)
    : IRequestHandler<GetCoachBookingsQuery, Result<PagedResult<BookingSessionDto>>>
{
    public async Task<Result<PagedResult<BookingSessionDto>>> Handle(GetCoachBookingsQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.GetByCoachUserIdAsync(request.CoachUserId, request.Page, request.PageSize, cancellationToken);
        var dtos = new PagedResult<BookingSessionDto>(
            paged.Items.Select(GetBookingQueryHandler.ToDto).ToList(),
            paged.TotalCount, paged.Page, paged.PageSize);
        return Result.Success(dtos);
    }
}
