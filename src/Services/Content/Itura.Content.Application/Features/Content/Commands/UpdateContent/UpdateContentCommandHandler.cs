using Itura.Content.Application.Common.Interfaces;
using Itura.Content.Application.DTOs;
using Itura.Content.Application.Features.Content.Queries.GetContent;
using Itura.Content.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Content.Application.Features.Content.Commands.UpdateContent;

internal sealed class UpdateContentCommandHandler(
    IContentRepository repository,
    IContentUnitOfWork unitOfWork)
    : IRequestHandler<UpdateContentCommand, Result<ContentItemDto>>
{
    public async Task<Result<ContentItemDto>> Handle(UpdateContentCommand request, CancellationToken cancellationToken)
    {
        var item = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (item is null)
            return Result.Failure<ContentItemDto>(Error.NotFound("ContentItem", request.Id));

        if (item.AuthorUserId != request.AuthorUserId)
            return Result.Failure<ContentItemDto>(Error.Forbidden("Only the author can update this content."));

        var result = item.Update(
            request.Title,
            request.Description,
            request.Body,
            request.Tags,
            request.ThumbnailUrl,
            request.MediaUrl,
            request.DurationSeconds);

        if (result.IsFailure)
            return Result.Failure<ContentItemDto>(result.Error);

        repository.Update(item);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return GetContentQueryHandler.ToDto(item);
    }
}
