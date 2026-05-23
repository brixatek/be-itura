using Itura.Notification.Application.DTOs;
using Itura.Notification.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Notification.Application.Features.Notifications.GetNotifications;

internal sealed class GetNotificationsQueryHandler(INotificationRepository repository)
    : IRequestHandler<GetNotificationsQuery, Result<PagedResult<NotificationDto>>>
{
    public async Task<Result<PagedResult<NotificationDto>>> Handle(
        GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await repository.GetByUserIdAsync(
            request.UserId, request.Page, request.PageSize, cancellationToken);

        var dtos = items.Select(n => new NotificationDto(
            n.Id, n.UserId, n.Title, n.Body,
            n.Type.ToString(), n.Channel.ToString(),
            n.IsRead, n.ReadAt, n.CreatedAt)).ToList();

        return Result.Success(new PagedResult<NotificationDto>(dtos, total, request.Page, request.PageSize));
    }
}
