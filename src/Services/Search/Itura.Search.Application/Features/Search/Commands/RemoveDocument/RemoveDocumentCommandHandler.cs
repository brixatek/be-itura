using Itura.Search.Application.Common.Interfaces;
using Itura.Search.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Search.Application.Features.Search.Commands.RemoveDocument;

internal sealed class RemoveDocumentCommandHandler(
    ISearchDocumentRepository repository,
    ISearchUnitOfWork unitOfWork)
    : IRequestHandler<RemoveDocumentCommand, Result>
{
    public async Task<Result> Handle(RemoveDocumentCommand request, CancellationToken cancellationToken)
    {
        var doc = await repository.GetByEntityAsync(request.EntityType, request.EntityId, cancellationToken);
        if (doc is null)
            return Result.Failure(Error.NotFound("SearchDocument", $"{request.EntityType}/{request.EntityId}"));
        doc.Deactivate();
        repository.Update(doc);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
