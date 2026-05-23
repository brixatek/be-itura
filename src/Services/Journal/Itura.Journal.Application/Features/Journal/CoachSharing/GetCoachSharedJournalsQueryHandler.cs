using Itura.Journal.Application.Common.Interfaces;
using Itura.Journal.Application.DTOs;
using Itura.Journal.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Journal.Application.Features.Journal.CoachSharing;

internal sealed class GetCoachSharedJournalsQueryHandler(
    IJournalCoachShareRepository shareRepository,
    IEncryptionService encryption)
    : IRequestHandler<GetCoachSharedJournalsQuery, Result<PagedResult<JournalEntryDto>>>
{
    public async Task<Result<PagedResult<JournalEntryDto>>> Handle(
        GetCoachSharedJournalsQuery request, CancellationToken cancellationToken)
    {
        var entries = await shareRepository.GetSharedWithCoachAsync(
            request.CoachId, request.Page, request.PageSize, cancellationToken);

        var dtos = entries.Select(e => new JournalEntryDto(
            e.Id, e.UserId, e.Title, encryption.Decrypt(e.Content),
            e.Tags, e.MoodScore, e.IsPrivate, e.CreatedAt, e.UpdatedAt)).ToList();

        return Result.Success(new PagedResult<JournalEntryDto>(
            dtos, dtos.Count, request.Page, request.PageSize));
    }
}
