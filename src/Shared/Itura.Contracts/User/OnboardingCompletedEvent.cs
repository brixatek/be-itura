namespace Itura.Contracts.User;

public sealed record OnboardingCompletedEvent(
    Guid AccountId,
    DateTime CompletedAt);
