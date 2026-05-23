using Itura.Mood.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Mood.Application.Features.Mood.GetHistory;

public sealed record GetMoodHistoryQuery(
    Guid UserId,
    int Page = 1,
    int PageSize = 20,
    DateTime? From = null,
    DateTime? To = null) : IRequest<Result<PagedResult<MoodEntryDto>>>;
