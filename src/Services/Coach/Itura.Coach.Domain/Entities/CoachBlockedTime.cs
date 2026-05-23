using Itura.SharedKernel.Entities;

namespace Itura.Coach.Domain.Entities;

public sealed class CoachBlockedTime : AuditableEntity
{
    public Guid CoachUserId { get; private set; }
    public DateTime StartUtc { get; private set; }
    public DateTime EndUtc { get; private set; }
    public string? Reason { get; private set; }

    private CoachBlockedTime() { }

    public static CoachBlockedTime Create(Guid coachUserId, DateTime startUtc, DateTime endUtc, string? reason = null)
    {
        return new CoachBlockedTime
        {
            Id = Guid.NewGuid(),
            CoachUserId = coachUserId,
            StartUtc = startUtc,
            EndUtc = endUtc,
            Reason = reason
        };
    }
}
