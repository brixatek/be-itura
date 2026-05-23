using Itura.SharedKernel.Domain;

namespace Itura.AI.Domain.Entities;

public sealed class UserRecommendation : AggregateRoot
{
    public Guid UserId { get; private set; }
    public string RecommendationType { get; private set; } = string.Empty;
    public string EntityType { get; private set; } = string.Empty;
    public Guid EntityId { get; private set; }
    public float Score { get; private set; }
    public string? Reason { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsActive { get; private set; }

    private UserRecommendation() { }

    public static UserRecommendation Create(
        Guid userId,
        string recommendationType,
        string entityType,
        Guid entityId,
        float score,
        string? reason,
        TimeSpan? ttl = null)
    {
        return new UserRecommendation
        {
            UserId = userId,
            RecommendationType = recommendationType,
            EntityType = entityType,
            EntityId = entityId,
            Score = score,
            Reason = reason,
            ExpiresAt = DateTime.UtcNow.Add(ttl ?? TimeSpan.FromDays(7)),
            IsActive = true
        };
    }

    public void Expire()
    {
        IsActive = false;
        MarkUpdated();
    }
}
