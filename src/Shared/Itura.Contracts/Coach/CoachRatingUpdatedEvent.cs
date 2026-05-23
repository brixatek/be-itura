namespace Itura.Contracts.Coach;

public sealed record CoachRatingUpdatedEvent(
    Guid CoachId,
    double NewAverageRating,
    int TotalReviews,
    DateTime OccurredAt);
