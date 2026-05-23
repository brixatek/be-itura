using Itura.Notification.Application.Common.Interfaces;
using Itura.Notification.Domain.Entities;
using Itura.Notification.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Notification.Application.Features.Notifications.Create;

internal sealed class CreateNotificationCommandHandler(
    INotificationRepository repository,
    INotificationUnitOfWork unitOfWork)
    : IRequestHandler<CreateNotificationCommand, Result>
{
    public async Task<Result> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
    {
        var notification = UserNotification.Create(
            request.UserId, request.Title, request.Body, request.Type, request.Channel);

        await repository.AddAsync(notification, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
