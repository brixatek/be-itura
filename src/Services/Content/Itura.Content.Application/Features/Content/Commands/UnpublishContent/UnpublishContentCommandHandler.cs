using Itura.Content.Application.Common.Interfaces;
using Itura.Content.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Content.Application.Features.Content.Commands.UnpublishContent;

internal sealed class UnpublishContentCommandHandler(
    IContentRepository repository,
    IContentUnitOfWork unitOfWork)
    : IRequestHandler<UnpublishContentCommand, Result>
{
    public async Task<Result> Handle(UnpublishContentCommand request, CancellationToken cancellationToken)
    {
        var item = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (item is null)
            return Result.Failure(Error.NotFound("ContentItem", request.Id));

        if (item.AuthorUserId != request.AuthorUserId)
            return Result.Failure(Error.Forbidden("Only the author can unpublish this content."));

        var result = item.Unpublish();
        if (result.IsFailure) return result;

        repository.Update(item);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
