using Itura.Notification.Application.Common.Interfaces;
using Itura.Notification.Domain.Entities;
using Itura.Notification.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Notification.Application.Features.Notifications.DeviceTokens;

public sealed record RegisterDeviceTokenCommand(Guid UserId, string Token, string Platform) : IRequest<Result>;

internal sealed class RegisterDeviceTokenCommandHandler(
    IDeviceTokenRepository repository,
    INotificationUnitOfWork unitOfWork) : IRequestHandler<RegisterDeviceTokenCommand, Result>
{
    public async Task<Result> Handle(RegisterDeviceTokenCommand request, CancellationToken cancellationToken)
    {
        var existing = await repository.GetByTokenAsync(request.Token, cancellationToken);
        if (existing is not null)
        {
            if (!existing.IsActive) { existing.Deactivate(); /* reactivate below */ }
            return Result.Success();
        }

        var token = DeviceToken.Create(request.UserId, request.Token, request.Platform);
        await repository.AddAsync(token, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
