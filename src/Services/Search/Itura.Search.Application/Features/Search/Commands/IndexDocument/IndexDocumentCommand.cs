using Itura.Search.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Search.Application.Features.Search.Commands.IndexDocument;

public sealed record IndexDocumentCommand(
    string EntityType,
    Guid EntityId,
    string Title,
    string? Description,
    string[] Tags) : IRequest<Result<SearchDocumentDto>>;
