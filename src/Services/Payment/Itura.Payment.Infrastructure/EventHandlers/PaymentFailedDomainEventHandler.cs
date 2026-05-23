using Itura.Contracts.Payment;
using Itura.Payment.Domain.Events;
using MassTransit;
using MediatR;

namespace Itura.Payment.Infrastructure.EventHandlers;

internal sealed class PaymentFailedDomainEventHandler(IPublishEndpoint publishEndpoint)
    : INotificationHandler<PaymentFailedDomainEvent>
{
    public async Task Handle(PaymentFailedDomainEvent notification, CancellationToken cancellationToken)
    {
        await publishEndpoint.Publish(new PaymentFailedEvent(
            notification.PaymentId,
            notification.BookingId,
            notification.PayerUserId,
            notification.Amount,
            notification.Currency,
            notification.FailureReason,
            notification.OccurredAt), cancellationToken);
    }
}
