using Itura.SharedKernel.Entities;

namespace Itura.Journal.Domain.Entities;

public sealed class JournalCoachShare : AuditableEntity
{
    public Guid JournalEntryId { get; private set; }
    public Guid CoachId { get; private set; }
    public DateTime SharedAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public bool IsRevoked => RevokedAt.HasValue;

    private JournalCoachShare() { }

    public static JournalCoachShare Create(Guid journalEntryId, Guid coachId) =>
        new()
        {
            JournalEntryId = journalEntryId,
            CoachId = coachId,
            SharedAt = DateTime.UtcNow
        };

    public void Revoke()
    {
        RevokedAt = DateTime.UtcNow;
        MarkUpdated();
    }
}
