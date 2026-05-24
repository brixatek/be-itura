using Itura.Contracts.Booking;
using Itura.Payment.Domain.Entities;
using Itura.Payment.Infrastructure.Persistence;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Itura.Payment.Infrastructure.EventHandlers;

public sealed class BookingCompletedEventConsumer(
    PaymentDbContext dbContext,
    ILogger<BookingCompletedEventConsumer> logger) : IConsumer<BookingCompletedEvent>
{
    private const decimal DefaultCommissionRate = 0.20m;

    public async Task Consume(ConsumeContext<BookingCompletedEvent> context)
    {
        var msg = context.Message;

        logger.LogInformation("Recording coach earning for booking {BookingId}, coach {CoachUserId}",
            msg.BookingId, msg.CoachId);

        var earning = CoachEarning.Create(
            coachUserId: msg.CoachId,
            sessionId: msg.BookingId,
            bookingId: msg.BookingId,
            grossAmount: msg.Price,
            commissionRate: DefaultCommissionRate,
            currency: msg.Currency);

        await dbContext.CoachEarnings.AddAsync(earning, context.CancellationToken);
        await dbContext.SaveChangesAsync(context.CancellationToken);

        logger.LogInformation("Coach earning recorded: net {Net} {Currency} for coach {CoachUserId}",
            earning.NetAmount, earning.Currency, msg.CoachId);
    }
}
