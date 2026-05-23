using Itura.Contracts.Auth;
using Itura.Notification.Application.Common.Interfaces;
using Itura.Notification.Application.Features.Notifications.Create;
using Itura.Notification.Domain.Enums;
using MassTransit;
using MediatR;

namespace Itura.Notification.Infrastructure.Consumers;

public sealed class UserRegisteredEventConsumer(ISender sender, IEmailService emailService)
    : IConsumer<UserRegisteredEvent>
{
    public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
    {
        var msg = context.Message;

        await sender.Send(new CreateNotificationCommand(
            msg.AccountId,
            "Welcome to Itura!",
            $"Hi {msg.FullName}, your wellness journey starts now. Complete your profile to get personalised recommendations.",
            NotificationType.InApp,
            NotificationChannel.Welcome), context.CancellationToken);

        await emailService.SendAsync(
            msg.Email,
            msg.FullName,
            "Welcome to Itura",
            $"""
            <h2>Welcome to Itura, {msg.FullName}!</h2>
            <p>We're excited to have you on your wellness journey.</p>
            <p>Head over to the app to complete your profile and get started.</p>
            """,
            context.CancellationToken);
    }
}
