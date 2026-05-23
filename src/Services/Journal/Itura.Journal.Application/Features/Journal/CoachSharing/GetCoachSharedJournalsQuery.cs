using Itura.Journal.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Journal.Application.Features.Journal.CoachSharing;

public sealed record GetCoachSharedJournalsQuery(Guid CoachId, int Page = 1, int PageSize = 20)
    : IRequest<Result<PagedResult<JournalEntryDto>>>;
