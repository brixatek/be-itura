using Itura.Booking.Application.Common.Interfaces;
using Itura.Booking.Domain.Repositories;
using Itura.Contracts.Payment;
using Itura.SharedKernel.Results;
using MassTransit;
using MediatR;

namespace Itura.Booking.Application.Features.Bookings.CancelBooking;

internal sealed class CancelBookingCommandHandler(
    IBookingRepository repository,
    IBookingUnitOfWork unitOfWork,
    IPublishEndpoint publishEndpoint)
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

        var (refundAmount, refundTier) = CalculateRefund(booking.Price, booking.ScheduledAt);
        if (refundAmount > 0)
        {
            await publishEndpoint.Publish(new BookingRefundRequestedEvent(
                booking.Id,
                booking.ClientUserId,
                booking.CoachUserId,
                refundAmount,
                booking.Currency,
                refundTier,
                DateTime.UtcNow), cancellationToken);
        }

        return Result.Success();
    }

    private static (decimal Amount, string Tier) CalculateRefund(decimal price, DateTime scheduledAt)
    {
        var hoursUntilSession = (scheduledAt - DateTime.UtcNow).TotalHours;

        if (hoursUntilSession > 24)
            return (price, "full");

        if (hoursUntilSession >= 2)
            return (Math.Round(price * 0.5m, 2), "half");

        return (0m, "none");
    }
}
