namespace Itura.SharedKernel.Entities;

public abstract class AuditableEntity : BaseEntity
{
    public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; protected set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; protected set; }

    public bool IsDeleted => DeletedAt.HasValue;

    protected void MarkUpdated() => UpdatedAt = DateTime.UtcNow;
    protected void MarkDeleted() => DeletedAt = DateTime.UtcNow;
}
