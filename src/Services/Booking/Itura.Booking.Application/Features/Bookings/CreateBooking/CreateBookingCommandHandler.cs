using Itura.Booking.Application.Common.Interfaces;
using Itura.Booking.Domain.Entities;
using Itura.Booking.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Booking.Application.Features.Bookings.CreateBooking;

internal sealed class CreateBookingCommandHandler(
    IBookingRepository repository,
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

        await repository.AddAsync(result.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(result.Value.Id);
    }
}
