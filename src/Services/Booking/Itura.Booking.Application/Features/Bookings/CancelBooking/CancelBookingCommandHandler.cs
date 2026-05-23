using Itura.Booking.Application.Common.Interfaces;
using Itura.Booking.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Booking.Application.Features.Bookings.CancelBooking;

internal sealed class CancelBookingCommandHandler(
    IBookingRepository repository,
    IBookingUnitOfWork unitOfWork)
    : IRequestHandler<CancelBookingCommand, Result>
{
    public async Task<Result> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await repository.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking is null)
            return Result.Failure(Error.NotFound("Booking", request.BookingId));

        if (booking.ClientUserId != request.UserId && booking.CoachUserId != request.UserId)
            return Result.Failure(Error.Forbidden());

        var result = booking.Cancel(request.Reason);
        if (result.IsFailure) return result;

        repository.Update(booking);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
