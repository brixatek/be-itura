using Itura.Notification.Application.Common.Interfaces;
using Itura.Notification.Domain.Entities;
using Itura.Notification.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Notification.Application.Features.Notifications.UpdatePreferences;

internal sealed class UpdateNotificationPreferencesCommandHandler(
    INotificationPreferenceRepository preferenceRepository,
    INotificationUnitOfWork unitOfWork)
    : IRequestHandler<UpdateNotificationPreferencesCommand, Result>
{
    public async Task<Result> Handle(UpdateNotificationPreferencesCommand request, CancellationToken cancellationToken)
    {
        var pref = await preferenceRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        if (pref is null)
        {
            pref = NotificationPreference.Create(request.UserId);
            await preferenceRepository.AddAsync(pref, cancellationToken);
        }

        pref.Update(request.PushEnabled, request.EmailEnabled, request.QuietHoursStart, request.QuietHoursEnd);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
