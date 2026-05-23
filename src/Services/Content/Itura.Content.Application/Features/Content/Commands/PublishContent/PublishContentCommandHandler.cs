using Itura.Content.Application.Common.Interfaces;
using Itura.Content.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Content.Application.Features.Content.Commands.PublishContent;

internal sealed class PublishContentCommandHandler(
    IContentRepository repository,
    IContentUnitOfWork unitOfWork)
    : IRequestHandler<PublishContentCommand, Result>
{
    public async Task<Result> Handle(PublishContentCommand request, CancellationToken cancellationToken)
    {
        var item = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (item is null)
            return Result.Failure(Error.NotFound("ContentItem", request.Id));

        if (item.AuthorUserId != request.AuthorUserId)
            return Result.Failure(Error.Forbidden("Only the author can publish this content."));

        var result = item.Publish();
        if (result.IsFailure) return result;

        repository.Update(item);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
