using Itura.Content.Application.DTOs;
using Itura.Content.Application.Features.Content.Queries.GetContent;
using Itura.Content.Domain.Enums;
using Itura.Content.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Content.Application.Features.Content.Queries.GetContents;

internal sealed class GetContentsQueryHandler(IContentRepository repository)
    : IRequestHandler<GetContentsQuery, Result<PagedResult<ContentItemDto>>>
{
    public async Task<Result<PagedResult<ContentItemDto>>> Handle(GetContentsQuery request, CancellationToken cancellationToken)
    {
        ContentType? contentType = null;
        if (!string.IsNullOrWhiteSpace(request.ContentType))
        {
            if (!Enum.TryParse<ContentType>(request.ContentType, true, out var parsed))
                return Result.Failure<PagedResult<ContentItemDto>>(
                    Error.Validation("Content.ContentType", "Invalid content type."));
            contentType = parsed;
        }

        var paged = await repository.GetAsync(
            request.Page,
            request.PageSize,
            contentType,
            request.Tag,
            request.AuthorUserId,
            request.IsPublished,
            cancellationToken);

        var dtos = paged.Items.Select(GetContentQueryHandler.ToDto).ToList();
        return new PagedResult<ContentItemDto>(dtos, paged.TotalCount, paged.Page, paged.PageSize);
    }
}
