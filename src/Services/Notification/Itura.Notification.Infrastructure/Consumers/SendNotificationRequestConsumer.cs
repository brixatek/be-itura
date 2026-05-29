using Itura.Contracts.Notification;
using Itura.Notification.Application.Common.Interfaces;
using Itura.Notification.Application.Features.Notifications.Create;
using Itura.Notification.Domain.Enums;
using Itura.Notification.Domain.Repositories;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Itura.Notification.Infrastructure.Consumers;

public sealed class SendNotificationRequestConsumer(
    ISender sender,
    IPushNotificationService pushService,
    IEmailService emailService,
    INotificationPreferenceRepository preferenceRepository,
    ILogger<SendNotificationRequestConsumer> logger) : IConsumer<SendNotificationRequest>
{
    public async Task Consume(ConsumeContext<SendNotificationRequest> context)
    {
        var msg = context.Message;
        var ct = context.CancellationToken;

        if (!Enum.TryParse<NotificationChannel>(msg.Channel, ignoreCase: true, out var channel))
            channel = NotificationChannel.SystemAlert;

        // Always store in-app
        await sender.Send(new CreateNotificationCommand(
            msg.UserId, msg.Title, msg.Body,
            NotificationType.InApp, channel), ct);

        // Email channel — dispatch email when recipient address is provided
        if (msg.Channel.Equals("Email", StringComparison.OrdinalIgnoreCase))
        {
            if (!string.IsNullOrEmpty(msg.RecipientEmail))
            {
                await emailService.SendAsync(
                    msg.RecipientEmail,
                    msg.RecipientName ?? string.Empty,
                    msg.Title,
                    $"<p>{msg.Body}</p>",
                    ct);
            }
            else
            {
                logger.LogWarning("Email notification requested for user {UserId} but no recipient email provided — skipped", msg.UserId);
            }
            return;
        }

        // Push channel — subject to user preferences and quiet hours
        if (msg.Channel.Equals("Push", StringComparison.OrdinalIgnoreCase))
        {
            var pref = await preferenceRepository.GetByUserIdAsync(msg.UserId, ct);

            if (pref is not null && !pref.PushEnabled)
            {
                logger.LogInformation("Push disabled for user {UserId} — skipped", msg.UserId);
                return;
            }

            if (pref is not null && pref.IsQuietHoursNow())
            {
                logger.LogInformation("Quiet hours active for user {UserId} — push skipped", msg.UserId);
                return;
            }

            var sent = await pushService.SendToUserAsync(msg.UserId, msg.Title, msg.Body, ct: ct);
            if (sent == 0)
                logger.LogInformation("No active device tokens for user {UserId} — push not delivered", msg.UserId);
        }
    }
}
