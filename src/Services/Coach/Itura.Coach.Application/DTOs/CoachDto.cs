namespace Itura.Coach.Application.DTOs;

public sealed record CoachDto(
    Guid Id,
    Guid UserId,
    string DisplayName,
    string Bio,
    IReadOnlyList<string> Specializations,
    IReadOnlyList<string> Languages,
    decimal HourlyRate,
    string Currency,
    string? ProfileImageUrl,
    int YearsOfExperience,
    bool IsActive,
    double? AverageRating,
    int TotalReviews,
    string VerificationStatus,
    DateTime? VerifiedAt,
    string? RejectionReason,
    DateTime CreatedAt,
    DateTime UpdatedAt);
