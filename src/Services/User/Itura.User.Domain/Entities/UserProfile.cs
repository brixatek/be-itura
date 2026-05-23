using Itura.SharedKernel.Domain;
using Itura.SharedKernel.Results;
using Itura.User.Domain.Enums;
using Itura.User.Domain.Events;

namespace Itura.User.Domain.Entities;

public sealed class UserProfile : AggregateRoot
{
    public string Email { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public string Timezone { get; private set; } = "UTC";
    public string? AvatarUrl { get; private set; }
    public string? Bio { get; private set; }
    public DateOnly? DateOfBirth { get; private set; }
    public Gender? Gender { get; private set; }
    public bool OnboardingCompleted { get; private set; }

    // XP & gamification
    public int TotalXp { get; private set; }
    public int WellnessLevel { get; private set; } = 1;

    // GDPR
    public bool IsAnonymized { get; private set; }

    // Preferences (embedded)
    public string Language { get; private set; } = "en";
    public bool EmailNotifications { get; private set; } = true;
    public bool PushNotifications { get; private set; } = true;
    public bool WeeklyDigest { get; private set; } = true;
    public string Theme { get; private set; } = "system";

    private readonly List<WellnessGoal> _wellnessGoals = [];
    public IReadOnlyCollection<WellnessGoal> WellnessGoals => _wellnessGoals.AsReadOnly();

    private UserProfile() { }

    public static UserProfile Create(Guid accountId, string email, string fullName, string timezone)
    {
        var profile = new UserProfile();
        profile.Id = accountId;
        profile.Email = email;
        profile.FullName = fullName;
        profile.Timezone = timezone;
        profile.RaiseDomainEvent(new UserProfileCreatedDomainEvent(accountId, email, fullName));
        return profile;
    }

    public void UpdateProfile(string fullName, string? avatarUrl, string? bio, DateOnly? dateOfBirth, Gender? gender, string timezone)
    {
        FullName = fullName;
        AvatarUrl = avatarUrl;
        Bio = bio;
        DateOfBirth = dateOfBirth;
        Gender = gender;
        Timezone = timezone;
    }

    public Result CompleteOnboarding(IEnumerable<string> goals)
    {
        if (OnboardingCompleted)
            return Result.Failure(Error.Conflict("User.OnboardingAlreadyCompleted", "Onboarding has already been completed."));

        _wellnessGoals.Clear();
        var priority = 1;
        foreach (var goal in goals)
            _wellnessGoals.Add(WellnessGoal.Create(Id, goal, priority++));

        OnboardingCompleted = true;
        RaiseDomainEvent(new OnboardingCompletedDomainEvent(Id));
        return Result.Success();
    }

    public void UpdatePreferences(bool emailNotifications, bool pushNotifications, bool weeklyDigest, string theme, string language)
    {
        EmailNotifications = emailNotifications;
        PushNotifications = pushNotifications;
        WeeklyDigest = weeklyDigest;
        Theme = theme;
        Language = language;
    }

    public int AwardXp(int amount)
    {
        TotalXp += amount;
        var newLevel = CalculateLevel(TotalXp);
        var leveled = newLevel > WellnessLevel;
        WellnessLevel = newLevel;
        return leveled ? newLevel : 0;
    }

    public void Anonymize()
    {
        Email = $"deleted_{Id:N}@anonymized.local";
        FullName = "Deleted User";
        AvatarUrl = null;
        Bio = null;
        DateOfBirth = null;
        Gender = null;
        IsAnonymized = true;
        MarkDeleted();
    }

    private static int CalculateLevel(int xp) => xp switch
    {
        < 100 => 1,
        < 300 => 2,
        < 600 => 3,
        < 1000 => 4,
        < 1500 => 5,
        < 2100 => 6,
        < 2800 => 7,
        < 3600 => 8,
        < 4500 => 9,
        _ => 10
    };
}
