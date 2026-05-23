namespace Itura.AI.Application.DTOs;

public sealed record RecommendationDto(
    Guid Id,
    Guid UserId,
    string RecommendationType,
    string EntityType,
    Guid EntityId,
    float Score,
    string? Reason,
    DateTime ExpiresAt);
