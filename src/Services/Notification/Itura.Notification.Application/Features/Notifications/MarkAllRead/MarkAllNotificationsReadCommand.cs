using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Notification.Application.Features.Notifications.MarkAllRead;

public sealed record MarkAllNotificationsReadCommand(Guid UserId) : IRequest<Result>;
