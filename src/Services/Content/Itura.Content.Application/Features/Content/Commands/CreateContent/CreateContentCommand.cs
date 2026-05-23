using Itura.Content.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Content.Application.Features.Content.Commands.CreateContent;

public sealed record CreateContentCommand(
    string Title,
    string? Description,
    string? Body,
    string ContentType,
    Guid AuthorUserId,
    List<string>? Tags,
    string? ThumbnailUrl,
    string? MediaUrl,
    int? DurationSeconds) : IRequest<Result<ContentItemDto>>;
