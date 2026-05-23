using Itura.Journal.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Journal.Application.Features.Journal.GetEntries;

public sealed record GetJournalEntriesQuery(
    Guid UserId,
    int Page,
    int PageSize,
    string? Tag = null,
    DateTime? From = null,
    DateTime? To = null) : IRequest<Result<PagedResult<JournalEntryDto>>>;
