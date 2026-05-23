using Itura.Search.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Search.Application.Features.Search.Queries.SearchDocuments;

public sealed record SearchDocumentsQuery(
    string Query,
    string? EntityType,
    int Page,
    int PageSize) : IRequest<Result<PagedResult<SearchDocumentDto>>>;
