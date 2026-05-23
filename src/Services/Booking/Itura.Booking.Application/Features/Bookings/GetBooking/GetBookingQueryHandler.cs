using Itura.Booking.Application.DTOs;
using Itura.Booking.Domain.Entities;
using Itura.Booking.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Booking.Application.Features.Bookings.GetBooking;

internal sealed class GetBookingQueryHandler(IBookingRepository repository)
    : IRequestHandler<GetBookingQuery, Result<BookingSessionDto>>
{
    public async Task<Result<BookingSessionDto>> Handle(GetBookingQuery request, CancellationToken cancellationToken)
    {
        var booking = await repository.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking is null)
            return Result.Failure<BookingSessionDto>(Error.NotFound("Booking", request.BookingId));

        if (booking.ClientUserId != request.UserId && booking.CoachUserId != request.UserId)
            return Result.Failure<BookingSessionDto>(Error.Forbidden());

        return Result.Success(ToDto(booking));
    }

    internal static BookingSessionDto ToDto(BookingSession b) => new(
        b.Id, b.CoachId, b.CoachUserId, b.ClientUserId,
        b.ScheduledAt, b.DurationMinutes,
        b.Status.ToString(), b.Price, b.Currency,
        b.CancellationReason, b.CreatedAt, b.UpdatedAt);
}
