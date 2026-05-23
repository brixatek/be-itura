namespace Itura.Search.Application.DTOs;

public sealed record SearchDocumentDto(
    Guid Id,
    string EntityType,
    Guid EntityId,
    string Title,
    string? Description,
    string[] Tags,
    DateTime UpdatedAt);
