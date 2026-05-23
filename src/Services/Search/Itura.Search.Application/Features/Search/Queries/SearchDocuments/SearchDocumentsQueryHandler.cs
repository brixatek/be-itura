using Itura.Search.Application.DTOs;
using Itura.Search.Application.Features.Search.Commands.IndexDocument;
using Itura.Search.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Search.Application.Features.Search.Queries.SearchDocuments;

internal sealed class SearchDocumentsQueryHandler(ISearchDocumentRepository repository)
    : IRequestHandler<SearchDocumentsQuery, Result<PagedResult<SearchDocumentDto>>>
{
    public async Task<Result<PagedResult<SearchDocumentDto>>> Handle(SearchDocumentsQuery request, CancellationToken cancellationToken)
    {
        var page = Math.Max(request.Page, 1);
        var pageSize = Math.Min(Math.Max(request.PageSize, 1), 50);
        var (items, total) = await repository.SearchAsync(request.Query, request.EntityType, page, pageSize, cancellationToken);
        var dtos = items.Select(IndexDocumentCommandHandler.ToDto);
        return new PagedResult<SearchDocumentDto>(dtos, total, page, pageSize);
    }
}
