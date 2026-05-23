using Itura.Booking.Application.DTOs;
using Itura.Booking.Application.Features.Bookings.GetBooking;
using Itura.Booking.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Booking.Application.Features.Bookings.GetMyBookings;

internal sealed class GetMyBookingsQueryHandler(IBookingRepository repository)
    : IRequestHandler<GetMyBookingsQuery, Result<PagedResult<BookingSessionDto>>>
{
    public async Task<Result<PagedResult<BookingSessionDto>>> Handle(GetMyBookingsQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.GetByClientUserIdAsync(request.ClientUserId, request.Page, request.PageSize, cancellationToken);
        var dtos = new PagedResult<BookingSessionDto>(
            paged.Items.Select(GetBookingQueryHandler.ToDto).ToList(),
            paged.TotalCount, paged.Page, paged.PageSize);
        return Result.Success(dtos);
    }
}
