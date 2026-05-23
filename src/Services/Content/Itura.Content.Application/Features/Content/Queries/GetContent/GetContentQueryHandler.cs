using Itura.Content.Application.DTOs;
using Itura.Content.Domain.Entities;
using Itura.Content.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Content.Application.Features.Content.Queries.GetContent;

internal sealed class GetContentQueryHandler(IContentRepository repository)
    : IRequestHandler<GetContentQuery, Result<ContentItemDto>>
{
    public async Task<Result<ContentItemDto>> Handle(GetContentQuery request, CancellationToken cancellationToken)
    {
        var item = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (item is null)
            return Result.Failure<ContentItemDto>(Error.NotFound("ContentItem", request.Id));

        return ToDto(item);
    }

    internal static ContentItemDto ToDto(ContentItem item) => new(
        item.Id,
        item.Title,
        item.Slug,
        item.Description,
        item.Body,
        item.ContentType.ToString(),
        item.AuthorUserId,
        item.Tags,
        item.IsPublished,
        item.PublishedAt,
        item.ThumbnailUrl,
        item.MediaUrl,
        item.DurationSeconds,
        item.ViewCount,
        item.CreatedAt,
        item.UpdatedAt);
}
