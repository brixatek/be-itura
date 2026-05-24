namespace Itura.Journal.Application.DTOs;

public sealed record JournalTemplateDto(
    Guid Id,
    string Name,
    string Description,
    string Prompt,
    string Category);
