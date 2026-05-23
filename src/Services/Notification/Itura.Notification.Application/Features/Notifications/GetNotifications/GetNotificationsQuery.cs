using Itura.Notification.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Notification.Application.Features.Notifications.GetNotifications;

public sealed record GetNotificationsQuery(Guid UserId, int Page = 1, int PageSize = 20)
    : IRequest<Result<PagedResult<NotificationDto>>>;
