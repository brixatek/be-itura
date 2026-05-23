using Itura.Notification.Domain.Enums;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Notification.Application.Features.Notifications.Create;

public sealed record CreateNotificationCommand(
    Guid UserId,
    string Title,
    string Body,
    NotificationType Type,
    NotificationChannel Channel) : IRequest<Result>;
