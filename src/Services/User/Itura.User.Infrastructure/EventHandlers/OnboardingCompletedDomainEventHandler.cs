using Itura.Contracts.User;
using Itura.User.Domain.Events;
using MassTransit;
using MediatR;

namespace Itura.User.Infrastructure.EventHandlers;

internal sealed class OnboardingCompletedDomainEventHandler(IPublishEndpoint publishEndpoint)
    : INotificationHandler<OnboardingCompletedDomainEvent>
{
    public async Task Handle(OnboardingCompletedDomainEvent notification, CancellationToken cancellationToken)
    {
        await publishEndpoint.Publish(new OnboardingCompletedEvent(
            notification.AccountId,
            notification.OccurredAt), cancellationToken);
    }
}
