namespace Itura.Corporate.Application.DTOs;

public sealed record CorporateAccountDto(
    Guid Id,
    string CompanyName,
    Guid AdminUserId,
    string? Industry,
    string SubscriptionTier,
    int MaxEmployees,
    bool IsActive,
    string? LogoUrl,
    string? Website,
    DateTime CreatedAt,
    DateTime UpdatedAt);
