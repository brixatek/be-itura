namespace Itura.AI.Application.Common.Interfaces;

public interface IAIRateLimiter
{
    Task<RateLimitStatus> CheckAsync(Guid userId, int dailyLimit, CancellationToken ct = default);
}

public sealed record RateLimitStatus(bool IsAllowed, int Remaining, DateTime ResetAt);
