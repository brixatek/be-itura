using Itura.Content.Application.Common.Interfaces;
using Itura.Content.Application.DTOs;
using Itura.Content.Application.Features.Content.Queries.GetContent;
using Itura.Content.Domain.Enums;
using Itura.Content.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Content.Application.Features.Content.Commands.CreateContent;

internal sealed class CreateContentCommandHandler(
    IContentRepository repository,
    IContentUnitOfWork unitOfWork)
    : IRequestHandler<CreateContentCommand, Result<ContentItemDto>>
{
    public async Task<Result<ContentItemDto>> Handle(CreateContentCommand request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<ContentType>(request.ContentType, true, out var contentType))
            return Result.Failure<ContentItemDto>(Error.Validation("Content.ContentType", "Invalid content type."));

        var result = Domain.Entities.ContentItem.Create(
            request.Title,
            request.Description,
            request.Body,
            contentType,
            request.AuthorUserId,
            request.Tags,
            request.ThumbnailUrl,
            request.MediaUrl,
            request.DurationSeconds);

        if (result.IsFailure)
            return Result.Failure<ContentItemDto>(result.Error);

        await repository.AddAsync(result.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return GetContentQueryHandler.ToDto(result.Value);
    }
}
