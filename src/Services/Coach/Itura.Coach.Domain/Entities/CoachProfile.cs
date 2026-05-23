using Itura.Coach.Domain.Events;
using Itura.SharedKernel.Domain;
using Itura.SharedKernel.Results;

namespace Itura.Coach.Domain.Entities;

public sealed class CoachProfile : AggregateRoot
{
    public Guid UserId { get; private set; }
    public string DisplayName { get; private set; } = string.Empty;
    public string Bio { get; private set; } = string.Empty;
    public List<string> Specializations { get; private set; } = [];
    public List<string> Languages { get; private set; } = [];
    public decimal HourlyRate { get; private set; }
    public string Currency { get; private set; } = "USD";
    public string? ProfileImageUrl { get; private set; }
    public int YearsOfExperience { get; private set; }
    public bool IsActive { get; private set; }
    public double? AverageRating { get; private set; }
    public int TotalReviews { get; private set; }

    private CoachProfile() { }

    public static Result<CoachProfile> Create(
        Guid userId, string displayName, string bio,
        IEnumerable<string> specializations, IEnumerable<string> languages,
        decimal hourlyRate, string currency, string? profileImageUrl, int yearsOfExperience)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            return Result.Failure<CoachProfile>(new Error("Coach.EmptyDisplayName", "Display name cannot be empty."));

        if (hourlyRate < 0)
            return Result.Failure<CoachProfile>(new Error("Coach.InvalidRate", "Hourly rate cannot be negative."));

        if (yearsOfExperience < 0)
            return Result.Failure<CoachProfile>(new Error("Coach.InvalidExperience", "Years of experience cannot be negative."));

        var coach = new CoachProfile
        {
            UserId = userId,
            DisplayName = displayName.Trim(),
            Bio = bio?.Trim() ?? string.Empty,
            Specializations = specializations.Select(s => s.Trim().ToLowerInvariant()).Distinct().Where(s => s.Length > 0).ToList(),
            Languages = languages.Select(l => l.Trim().ToLowerInvariant()).Distinct().Where(l => l.Length > 0).ToList(),
            HourlyRate = hourlyRate,
            Currency = currency?.Trim().ToUpperInvariant() ?? "USD",
            ProfileImageUrl = profileImageUrl?.Trim(),
            YearsOfExperience = yearsOfExperience,
            IsActive = true
        };

        coach.RaiseDomainEvent(new CoachProfileCreatedDomainEvent(
            coach.Id, userId, coach.DisplayName,
            coach.Specializations, coach.Languages,
            coach.HourlyRate, coach.Currency, coach.CreatedAt));

        return Result.Success(coach);
    }

    public Result UpdateProfile(
        string displayName, string bio,
        IEnumerable<string> specializations, IEnumerable<string> languages,
        decimal hourlyRate, string currency, string? profileImageUrl, int yearsOfExperience)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            return Result.Failure(new Error("Coach.EmptyDisplayName", "Display name cannot be empty."));

        if (hourlyRate < 0)
            return Result.Failure(new Error("Coach.InvalidRate", "Hourly rate cannot be negative."));

        if (yearsOfExperience < 0)
            return Result.Failure(new Error("Coach.InvalidExperience", "Years of experience cannot be negative."));

        DisplayName = displayName.Trim();
        Bio = bio?.Trim() ?? string.Empty;
        Specializations = specializations.Select(s => s.Trim().ToLowerInvariant()).Distinct().Where(s => s.Length > 0).ToList();
        Languages = languages.Select(l => l.Trim().ToLowerInvariant()).Distinct().Where(l => l.Length > 0).ToList();
        HourlyRate = hourlyRate;
        Currency = currency?.Trim().ToUpperInvariant() ?? "USD";
        ProfileImageUrl = profileImageUrl?.Trim();
        YearsOfExperience = yearsOfExperience;
        MarkUpdated();

        return Result.Success();
    }

    public void UpdateRating(double newAverageRating, int totalReviews)
    {
        AverageRating = newAverageRating;
        TotalReviews = totalReviews;
        MarkUpdated();
        RaiseDomainEvent(new CoachRatingUpdatedDomainEvent(Id, newAverageRating, totalReviews, UpdatedAt));
    }

    public void Deactivate()
    {
        IsActive = false;
        MarkUpdated();
    }

    public void Activate()
    {
        IsActive = true;
        MarkUpdated();
    }
}
