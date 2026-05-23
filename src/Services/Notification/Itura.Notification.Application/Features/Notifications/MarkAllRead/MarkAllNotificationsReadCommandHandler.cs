using Itura.Notification.Application.Common.Interfaces;
using Itura.Notification.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Notification.Application.Features.Notifications.MarkAllRead;

internal sealed class MarkAllNotificationsReadCommandHandler(
    INotificationRepository repository,
    INotificationUnitOfWork unitOfWork)
    : IRequestHandler<MarkAllNotificationsReadCommand, Result>
{
    public async Task<Result> Handle(MarkAllNotificationsReadCommand request, CancellationToken cancellationToken)
    {
        var unread = await repository.GetUnreadByUserIdAsync(request.UserId, cancellationToken);
        var list = unread.ToList();

        if (list.Count == 0) return Result.Success();

        foreach (var n in list) n.MarkAsRead();
        repository.UpdateRange(list);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
