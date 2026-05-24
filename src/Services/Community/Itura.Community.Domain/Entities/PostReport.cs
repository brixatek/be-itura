using Itura.SharedKernel.Entities;

namespace Itura.Community.Domain.Entities;

public sealed class PostReport : AuditableEntity
{
    public Guid PostId { get; private set; }
    public Guid ReporterUserId { get; private set; }
    public string Reason { get; private set; } = string.Empty;
    public bool IsReviewed { get; private set; }
    public DateTime? ReviewedAt { get; private set; }

    private PostReport() { }

    public static PostReport Create(Guid postId, Guid reporterUserId, string reason) =>
        new()
        {
            Id = Guid.NewGuid(),
            PostId = postId,
            ReporterUserId = reporterUserId,
            Reason = reason.Trim()
        };

    public void MarkReviewed()
    {
        IsReviewed = true;
        ReviewedAt = DateTime.UtcNow;
        MarkUpdated();
    }
}
