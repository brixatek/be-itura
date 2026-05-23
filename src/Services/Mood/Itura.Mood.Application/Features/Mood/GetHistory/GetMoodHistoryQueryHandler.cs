using Itura.Mood.Application.DTOs;
using Itura.Mood.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Mood.Application.Features.Mood.GetHistory;

internal sealed class GetMoodHistoryQueryHandler(IMoodEntryRepository repository)
    : IRequestHandler<GetMoodHistoryQuery, Result<PagedResult<MoodEntryDto>>>
{
    public async Task<Result<PagedResult<MoodEntryDto>>> Handle(
        GetMoodHistoryQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await repository.GetByUserIdAsync(
            request.UserId, request.Page, request.PageSize,
            request.From, request.To, cancellationToken);

        var dtos = items.Select(e => new MoodEntryDto(
            e.Id, e.Score, e.Note, e.Emotions.AsReadOnly(),
            e.LoggedAt, e.CreatedAt)).ToList();

        return Result.Success(new PagedResult<MoodEntryDto>(dtos, total, request.Page, request.PageSize));
    }
}
