using Itura.Booking.Domain.Events;
using Itura.Contracts.Booking;
using MassTransit;
using MediatR;

namespace Itura.Booking.Infrastructure.EventHandlers;

internal sealed class BookingConfirmedDomainEventHandler(IPublishEndpoint publishEndpoint)
    : INotificationHandler<BookingConfirmedDomainEvent>
{
    public async Task Handle(BookingConfirmedDomainEvent notification, CancellationToken cancellationToken)
    {
        await publishEndpoint.Publish(new BookingConfirmedEvent(
            notification.BookingId,
            notification.CoachId,
            notification.ClientUserId,
            notification.ScheduledAt,
            notification.DurationMinutes,
            notification.Price,
            notification.Currency,
            notification.OccurredAt), cancellationToken);
    }
}
