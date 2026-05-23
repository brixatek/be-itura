namespace Itura.Contracts.Coach;

public sealed record CoachProfileCreatedEvent(
    Guid CoachId,
    Guid UserId,
    string DisplayName,
    IReadOnlyList<string> Specializations,
    IReadOnlyList<string> Languages,
    decimal HourlyRate,
    string Currency,
    DateTime CreatedAt);
