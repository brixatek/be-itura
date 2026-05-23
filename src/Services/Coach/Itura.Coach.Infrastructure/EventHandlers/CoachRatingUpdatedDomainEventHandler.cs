using Itura.Coach.Domain.Events;
using Itura.Contracts.Coach;
using MassTransit;
using MediatR;

namespace Itura.Coach.Infrastructure.EventHandlers;

internal sealed class CoachRatingUpdatedDomainEventHandler(IPublishEndpoint publishEndpoint)
    : INotificationHandler<CoachRatingUpdatedDomainEvent>
{
    public async Task Handle(CoachRatingUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        await publishEndpoint.Publish(new CoachRatingUpdatedEvent(
            notification.CoachId,
            notification.NewAverageRating,
            notification.TotalReviews,
            notification.RatingChangedAt), cancellationToken);
    }
}
