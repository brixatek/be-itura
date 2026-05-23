using Itura.SharedKernel.Domain;

namespace Itura.Coach.Domain.Events;

public sealed record CoachProfileCreatedDomainEvent(
    Guid CoachId,
    Guid UserId,
    string DisplayName,
    IReadOnlyList<string> Specializations,
    IReadOnlyList<string> Languages,
    decimal HourlyRate,
    string Currency,
    DateTime CreatedAt) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
