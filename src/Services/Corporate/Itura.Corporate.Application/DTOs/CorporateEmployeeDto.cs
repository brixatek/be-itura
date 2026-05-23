namespace Itura.Corporate.Application.DTOs;

public sealed record CorporateEmployeeDto(
    Guid Id,
    Guid CorporateAccountId,
    Guid UserId,
    string Role,
    bool IsActive,
    DateTime CreatedAt);
