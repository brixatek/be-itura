using Itura.Notification.Application.Common.Interfaces;
using Itura.Notification.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Notification.Application.Features.Notifications.MarkRead;

internal sealed class MarkNotificationReadCommandHandler(
    INotificationRepository repository,
    INotificationUnitOfWork unitOfWork)
    : IRequestHandler<MarkNotificationReadCommand, Result>
{
    public async Task<Result> Handle(MarkNotificationReadCommand request, CancellationToken cancellationToken)
    {
        var notification = await repository.GetByIdAsync(request.NotificationId, cancellationToken);

        if (notification is null || notification.UserId != request.UserId)
            return Result.Failure(new Error("Notification.NotFound", "Notification not found."));

        notification.MarkAsRead();
        repository.Update(notification);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
