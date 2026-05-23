namespace Itura.Mood.Application.DTOs;

public sealed record MoodEntryDto(
    Guid Id,
    int Score,
    string? Note,
    IReadOnlyList<string> Emotions,
    DateTime LoggedAt,
    DateTime CreatedAt);

public sealed record MoodSummaryDto(
    double AverageScore,
    int TotalEntries,
    int DaysTracked,
    string Trend,
    double? ThisWeekAverage,
    double? LastWeekAverage,
    IReadOnlyList<EmotionCountDto> TopEmotions);

public sealed record EmotionCountDto(string Emotion, int Count);
