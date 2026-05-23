using Itura.Content.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Content.Application.Features.Content.Commands.UpdateContent;

public sealed record UpdateContentCommand(
    Guid Id,
    Guid AuthorUserId,
    string Title,
    string? Description,
    string? Body,
    List<string>? Tags,
    string? ThumbnailUrl,
    string? MediaUrl,
    int? DurationSeconds) : IRequest<Result<ContentItemDto>>;
