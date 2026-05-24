namespace Itura.Contracts.AI;

public sealed record CrisisDetectedEvent(
    Guid UserId,
    string MessageSnippet,
    string Source,
    DateTime DetectedAt);
