using Itura.Search.Application.Common.Interfaces;
using Itura.Search.Application.DTOs;
using Itura.Search.Domain.Entities;
using Itura.Search.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Search.Application.Features.Search.Commands.IndexDocument;

internal sealed class IndexDocumentCommandHandler(
    ISearchDocumentRepository repository,
    ISearchUnitOfWork unitOfWork)
    : IRequestHandler<IndexDocumentCommand, Result<SearchDocumentDto>>
{
    public async Task<Result<SearchDocumentDto>> Handle(IndexDocumentCommand request, CancellationToken cancellationToken)
    {
        var existing = await repository.GetByEntityAsync(request.EntityType, request.EntityId, cancellationToken);
        if (existing is not null)
        {
            existing.Update(request.Title, request.Description, request.Tags);
            repository.Update(existing);
        }
        else
        {
            existing = SearchDocument.Create(request.EntityType, request.EntityId, request.Title, request.Description, request.Tags);
            await repository.AddAsync(existing, cancellationToken);
        }
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return ToDto(existing);
    }

    internal static SearchDocumentDto ToDto(SearchDocument d) =>
        new(d.Id, d.EntityType, d.EntityId, d.Title, d.Description, d.Tags, d.UpdatedAt);
}
