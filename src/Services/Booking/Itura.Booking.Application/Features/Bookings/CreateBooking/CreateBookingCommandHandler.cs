using Itura.Booking.Application.Common.Interfaces;
using Itura.Booking.Domain.Entities;
using Itura.Booking.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Booking.Application.Features.Bookings.CreateBooking;

internal sealed class CreateBookingCommandHandler(
    IBookingRepository repository,
    ISlotReservationService slotReservation,
    IBookingUnitOfWork unitOfWork)
    : IRequestHandler<CreateBookingCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        var result = BookingSession.Create(
            request.CoachId, request.CoachUserId, request.ClientUserId,
            request.ScheduledAt, request.DurationMinutes,
            request.Price, request.Currency);

        if (result.IsFailure) return Result.Failure<Guid>(result.Error);

        var booking = result.Value;

        var reserved = await slotReservation.TryReserveAsync(
            request.CoachUserId, request.ScheduledAt, booking.Id, cancellationToken);

        if (!reserved)
            return Result.Failure<Guid>(Error.Conflict("Booking.SlotTaken", "This time slot is no longer available."));

        await repository.AddAsync(booking, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(booking.Id);
    }
}
