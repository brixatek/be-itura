using Itura.Booking.Application.Common.Interfaces;
using Itura.Booking.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Booking.Application.Features.Bookings.GetCoachAvailability;

internal sealed class GetCoachAvailabilityQueryHandler(
    IBookingRepository repository,
    ICoachAvailabilityCache cache)
    : IRequestHandler<GetCoachAvailabilityQuery, Result<CoachAvailabilityDto>>
{
    public async Task<Result<CoachAvailabilityDto>> Handle(
        GetCoachAvailabilityQuery request, CancellationToken cancellationToken)
    {
        var cached = await cache.GetBookedSlotsAsync(request.CoachUserId, request.Date, cancellationToken);
        if (cached is not null)
            return Result.Success(new CoachAvailabilityDto(request.CoachUserId, request.Date, cached));

        var slots = await repository.GetBookedSlotsForDayAsync(request.CoachUserId, request.Date, cancellationToken);
        await cache.SetBookedSlotsAsync(request.CoachUserId, request.Date, slots, cancellationToken);

        return Result.Success(new CoachAvailabilityDto(request.CoachUserId, request.Date, slots));
    }
}
