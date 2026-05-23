using Itura.Contracts.Payment;
using Itura.Payment.Domain.Events;
using MassTransit;
using MediatR;

namespace Itura.Payment.Infrastructure.EventHandlers;

internal sealed class PaymentCompletedDomainEventHandler(IPublishEndpoint publishEndpoint)
    : INotificationHandler<PaymentCompletedDomainEvent>
{
    public async Task Handle(PaymentCompletedDomainEvent notification, CancellationToken cancellationToken)
    {
        await publishEndpoint.Publish(new PaymentCompletedEvent(
            notification.PaymentId,
            notification.BookingId,
            notification.PayerUserId,
            notification.PayeeUserId,
            notification.Amount,
            notification.Currency,
            notification.TransactionReference,
            notification.OccurredAt), cancellationToken);
    }
}
