using Itura.Notification.Application.Common.Interfaces;
using Itura.Notification.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Notification.Application.Features.Notifications.DeviceTokens;

public sealed record UnregisterDeviceTokenCommand(Guid UserId, string Token) : IRequest<Result>;

internal sealed class UnregisterDeviceTokenCommandHandler(
    IDeviceTokenRepository repository,
    INotificationUnitOfWork unitOfWork) : IRequestHandler<UnregisterDeviceTokenCommand, Result>
{
    public async Task<Result> Handle(UnregisterDeviceTokenCommand request, CancellationToken cancellationToken)
    {
        var existing = await repository.GetByTokenAsync(request.Token, cancellationToken);
        if (existing is null || existing.UserId != request.UserId)
            return Result.Failure(Error.NotFound("DeviceToken", request.Token));

        existing.Deactivate();
        repository.Update(existing);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
