namespace Itura.User.Application.DTOs;

public sealed record UserProfileDto(
    Guid Id,
    string Email,
    string FullName,
    string Timezone,
    string? AvatarUrl,
    string? Bio,
    DateOnly? DateOfBirth,
    string? Gender,
    bool OnboardingCompleted,
    IReadOnlyCollection<string> WellnessGoals,
    UserPreferencesDto Preferences);

public sealed record UserPreferencesDto(
    bool EmailNotifications,
    bool PushNotifications,
    bool WeeklyDigest,
    string Theme,
    string Language);
