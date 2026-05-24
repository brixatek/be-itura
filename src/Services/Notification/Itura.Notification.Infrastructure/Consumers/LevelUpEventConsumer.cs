using Itura.Contracts.Gamification;
using Itura.Notification.Application.Common.Interfaces;
using Itura.Notification.Application.Features.Notifications.Create;
using Itura.Notification.Domain.Enums;
using MassTransit;
using MediatR;

namespace Itura.Notification.Infrastructure.Consumers;

public sealed class LevelUpEventConsumer(ISender sender, IPushNotificationService pushService)
    : IConsumer<LevelUpEvent>
{
    public async Task Consume(ConsumeContext<LevelUpEvent> context)
    {
        var msg = context.Message;
        var ct = context.CancellationToken;

        await sender.Send(new CreateNotificationCommand(
            msg.UserId,
            "Level Up! 🎉",
            $"Congratulations! You've reached Wellness Level {msg.NewLevel}. Keep going!",
            NotificationType.InApp,
            NotificationChannel.LevelUp), ct);

        await pushService.SendToUserAsync(
            msg.UserId,
            "Level Up!",
            $"You've reached Wellness Level {msg.NewLevel}. Amazing progress!",
            new Dictionary<string, string> { ["type"] = "level_up", ["level"] = msg.NewLevel.ToString() },
            ct);
    }
}
