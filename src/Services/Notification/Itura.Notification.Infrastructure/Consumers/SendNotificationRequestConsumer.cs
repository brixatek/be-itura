using Itura.Contracts.Notification;
using Itura.Notification.Application.Features.Notifications.Create;
using Itura.Notification.Domain.Enums;
using MassTransit;
using MediatR;

namespace Itura.Notification.Infrastructure.Consumers;

public sealed class SendNotificationRequestConsumer(ISender sender) : IConsumer<SendNotificationRequest>
{
    public async Task Consume(ConsumeContext<SendNotificationRequest> context)
    {
        var msg = context.Message;

        if (!Enum.TryParse<NotificationChannel>(msg.Channel, ignoreCase: true, out var channel))
            channel = NotificationChannel.SystemAlert;

        await sender.Send(new CreateNotificationCommand(
            msg.UserId,
            msg.Title,
            msg.Body,
            NotificationType.InApp,
            channel), context.CancellationToken);
    }
}
