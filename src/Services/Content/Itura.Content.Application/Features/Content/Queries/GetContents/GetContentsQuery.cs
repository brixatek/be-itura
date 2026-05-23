using Itura.Content.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Content.Application.Features.Content.Queries.GetContents;

public sealed record GetContentsQuery(
    int Page,
    int PageSize,
    string? ContentType,
    string? Tag,
    Guid? AuthorUserId,
    bool? IsPublished) : IRequest<Result<PagedResult<ContentItemDto>>>;
