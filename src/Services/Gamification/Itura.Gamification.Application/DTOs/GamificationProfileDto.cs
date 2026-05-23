namespace Itura.Gamification.Application.DTOs;

public sealed record GamificationProfileDto(
    Guid Id,
    Guid UserId,
    int TotalPoints,
    int Level,
    int CurrentStreak,
    int LongestStreak,
    DateTime? LastActivityDate,
    DateTime CreatedAt);
