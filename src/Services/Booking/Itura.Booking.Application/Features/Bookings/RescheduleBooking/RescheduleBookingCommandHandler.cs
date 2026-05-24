using Itura.Booking.Application.Common.Interfaces;
using Itura.Booking.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Booking.Application.Features.Bookings.RescheduleBooking;

internal sealed class RescheduleBookingCommandHandler(
    IBookingRepository repository,
    IBookingUnitOfWork unitOfWork)
    : IRequestHandler<RescheduleBookingCommand, Result>
{
    public async Task<Result> Handle(RescheduleBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await repository.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking is null)
            return Result.Failure(Error.NotFound("Booking", request.BookingId));

        var result = booking.Reschedule(request.NewScheduledAt, request.RequestedByUserId);
        if (result.IsFailure) return result;

        repository.Update(booking);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
