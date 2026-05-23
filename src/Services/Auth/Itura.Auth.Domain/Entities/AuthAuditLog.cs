using Itura.SharedKernel.Entities;

namespace Itura.Auth.Domain.Entities;

public sealed class AuthAuditLog : BaseEntity
{
    public Guid AccountId { get; private set; }
    public string EventType { get; private set; } = string.Empty;
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public bool Success { get; private set; }
    public string? FailureReason { get; private set; }
    public DateTime OccurredAt { get; private set; }

    private AuthAuditLog() { }

    public static AuthAuditLog Create(
        Guid accountId, string eventType, bool success,
        string? ipAddress = null, string? userAgent = null, string? failureReason = null)
        => new()
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            EventType = eventType,
            Success = success,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            FailureReason = failureReason,
            OccurredAt = DateTime.UtcNow
        };
}
