using Itura.Contracts.Gamification;
using Itura.Notification.Application.Features.Notifications.Create;
using Itura.Notification.Domain.Enums;
using MassTransit;
using MediatR;

namespace Itura.Notification.Infrastructure.Consumers;

public sealed class BadgeEarnedEventConsumer(ISender sender) : IConsumer<BadgeEarnedEvent>
{
    public async Task Consume(ConsumeContext<BadgeEarnedEvent> context)
    {
        var msg = context.Message;
        var ct = context.CancellationToken;

        await sender.Send(new CreateNotificationCommand(
            msg.UserId,
            $"Badge Unlocked: {msg.BadgeName}",
            msg.BadgeDescription,
            NotificationType.InApp,
            NotificationChannel.BadgeEarned), ct);
    }
}
