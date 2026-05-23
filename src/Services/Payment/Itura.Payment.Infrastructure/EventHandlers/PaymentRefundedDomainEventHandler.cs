using Itura.Contracts.Payment;
using Itura.Payment.Domain.Events;
using MassTransit;
using MediatR;

namespace Itura.Payment.Infrastructure.EventHandlers;

internal sealed class PaymentRefundedDomainEventHandler(IPublishEndpoint publishEndpoint)
    : INotificationHandler<PaymentRefundedDomainEvent>
{
    public async Task Handle(PaymentRefundedDomainEvent notification, CancellationToken cancellationToken)
    {
        await publishEndpoint.Publish(new PaymentRefundedEvent(
            notification.PaymentId,
            notification.BookingId,
            notification.PayerUserId,
            notification.PayeeUserId,
            notification.Amount,
            notification.Currency,
            notification.OccurredAt), cancellationToken);
    }
}
