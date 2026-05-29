using Itura.Contracts.Payment;
using Itura.Payment.Application.Common.Interfaces;
using Itura.Payment.Domain.Repositories;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Itura.Payment.Infrastructure.EventHandlers;

public sealed class BookingRefundRequestedEventConsumer(
    IPaymentRepository paymentRepository,
    IPaymentUnitOfWork unitOfWork,
    ILogger<BookingRefundRequestedEventConsumer> logger) : IConsumer<BookingRefundRequestedEvent>
{
    public async Task Consume(ConsumeContext<BookingRefundRequestedEvent> context)
    {
        var msg = context.Message;
        var ct = context.CancellationToken;

        if (msg.RefundAmount <= 0)
        {
            logger.LogInformation("No refund due for booking {BookingId} (tier: {Tier})", msg.BookingId, msg.RefundTier);
            return;
        }

        var payment = await paymentRepository.GetByBookingIdAsync(msg.BookingId, ct);
        if (payment is null)
        {
            logger.LogWarning("Payment record not found for booking {BookingId} — refund skipped", msg.BookingId);
            return;
        }

        var result = payment.Refund(msg.RefundAmount);
        if (result.IsFailure)
        {
            logger.LogError("Refund failed for booking {BookingId}: {Error}", msg.BookingId, result.Error.Message);
            return;
        }

        paymentRepository.Update(payment);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("Refund of {Amount} {Currency} processed for booking {BookingId} (tier: {Tier})",
            msg.RefundAmount, msg.Currency, msg.BookingId, msg.RefundTier);
    }
}
