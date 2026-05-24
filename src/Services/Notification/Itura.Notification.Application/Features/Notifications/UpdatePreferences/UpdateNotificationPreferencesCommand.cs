using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Notification.Application.Features.Notifications.UpdatePreferences;

public sealed record UpdateNotificationPreferencesCommand(
    Guid UserId,
    bool PushEnabled,
    bool EmailEnabled,
    TimeOnly? QuietHoursStart,
    TimeOnly? QuietHoursEnd) : IRequest<Result>;
