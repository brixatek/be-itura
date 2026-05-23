using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.User.Application.Features.Users.UpdatePreferences;

public sealed record UpdatePreferencesCommand(
    Guid AccountId,
    bool EmailNotifications,
    bool PushNotifications,
    bool WeeklyDigest,
    string Theme,
    string Language) : IRequest<Result>;
