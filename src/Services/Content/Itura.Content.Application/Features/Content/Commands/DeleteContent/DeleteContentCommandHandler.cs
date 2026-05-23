using Itura.Content.Application.Common.Interfaces;
using Itura.Content.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Content.Application.Features.Content.Commands.DeleteContent;

internal sealed class DeleteContentCommandHandler(
    IContentRepository repository,
    IContentUnitOfWork unitOfWork)
    : IRequestHandler<DeleteContentCommand, Result>
{
    public async Task<Result> Handle(DeleteContentCommand request, CancellationToken cancellationToken)
    {
        var item = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (item is null)
            return Result.Failure(Error.NotFound("ContentItem", request.Id));

        if (item.AuthorUserId != request.AuthorUserId)
            return Result.Failure(Error.Forbidden("Only the author can delete this content."));

        item.Delete();
        repository.Update(item);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
