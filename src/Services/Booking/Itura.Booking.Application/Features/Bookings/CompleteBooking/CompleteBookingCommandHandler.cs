using Itura.Booking.Application.Common.Interfaces;
using Itura.Booking.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Booking.Application.Features.Bookings.CompleteBooking;

internal sealed class CompleteBookingCommandHandler(
    IBookingRepository repository,
    IBookingUnitOfWork unitOfWork)
    : IRequestHandler<CompleteBookingCommand, Result>
{
    public async Task<Result> Handle(CompleteBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await repository.GetByIdAsync(request.BookingId, cancellationToken);
        if (booking is null)
            return Result.Failure(Error.NotFound("Booking", request.BookingId));

        if (booking.CoachUserId != request.UserId)
            return Result.Failure(Error.Forbidden());

        var result = booking.Complete();
        if (result.IsFailure) return result;

        repository.Update(booking);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
