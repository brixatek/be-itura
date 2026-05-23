using Itura.Booking.Domain.Events;
using Itura.Contracts.Booking;
using MassTransit;
using MediatR;

namespace Itura.Booking.Infrastructure.EventHandlers;

internal sealed class BookingCreatedDomainEventHandler(IPublishEndpoint publishEndpoint)
    : INotificationHandler<BookingCreatedDomainEvent>
{
    public async Task Handle(BookingCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        await publishEndpoint.Publish(new BookingCreatedEvent(
            notification.BookingId,
            notification.CoachId,
            notification.CoachUserId,
            notification.ClientUserId,
            notification.ScheduledAt,
            notification.DurationMinutes,
            notification.Price,
            notification.Currency,
            notification.OccurredAt), cancellationToken);
    }
}
