using Itura.Payment.Domain.Events;
using MediatR;

namespace Itura.Payment.Infrastructure.EventHandlers;

internal sealed class PaymentInitiatedDomainEventHandler
    : INotificationHandler<PaymentInitiatedDomainEvent>
{
    public Task Handle(PaymentInitiatedDomainEvent notification, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}
