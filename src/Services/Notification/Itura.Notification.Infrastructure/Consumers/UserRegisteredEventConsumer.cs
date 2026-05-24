using Itura.Contracts.Auth;
using Itura.Notification.Application.Common.Interfaces;
using Itura.Notification.Application.Features.Notifications.Create;
using Itura.Notification.Domain.Entities;
using Itura.Notification.Domain.Enums;
using Itura.Notification.Domain.Repositories;
using MassTransit;
using MediatR;

namespace Itura.Notification.Infrastructure.Consumers;

public sealed class UserRegisteredEventConsumer(
    ISender sender,
    IEmailService emailService,
    INotificationPreferenceRepository preferenceRepository,
    INotificationUnitOfWork unitOfWork)
    : IConsumer<UserRegisteredEvent>
{
    public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
    {
        var msg = context.Message;
        var ct = context.CancellationToken;

        // Seed default notification preferences for the new user
        var existing = await preferenceRepository.GetByUserIdAsync(msg.AccountId, ct);
        if (existing is null)
        {
            var pref = NotificationPreference.Create(msg.AccountId);
            await preferenceRepository.AddAsync(pref, ct);
            await unitOfWork.SaveChangesAsync(ct);
        }

        await sender.Send(new CreateNotificationCommand(
            msg.AccountId,
            "Welcome to Itura!",
            $"Hi {msg.FullName}, your wellness journey starts now. Complete your profile to get personalised recommendations.",
            NotificationType.InApp,
            NotificationChannel.Welcome), ct);

        await emailService.SendAsync(
            msg.Email,
            msg.FullName,
            "Welcome to Itura",
            $"""
            <h2>Welcome to Itura, {msg.FullName}!</h2>
            <p>We're excited to have you on your wellness journey.</p>
            <p>Head over to the app to complete your profile and get started.</p>
            """,
            ct);
    }
}
