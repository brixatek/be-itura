using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Notification.Application.Features.Notifications.MarkRead;

public sealed record MarkNotificationReadCommand(Guid NotificationId, Guid UserId) : IRequest<Result>;
