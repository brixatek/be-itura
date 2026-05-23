using Itura.SharedKernel.Domain;

namespace Itura.Coach.Domain.Events;

public sealed record CoachRatingUpdatedDomainEvent(
    Guid CoachId,
    double NewAverageRating,
    int TotalReviews,
    DateTime RatingChangedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
