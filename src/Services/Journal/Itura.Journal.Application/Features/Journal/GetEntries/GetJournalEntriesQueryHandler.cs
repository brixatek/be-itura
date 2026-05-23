using Itura.Journal.Application.Common.Interfaces;
using Itura.Journal.Application.DTOs;
using Itura.Journal.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Journal.Application.Features.Journal.GetEntries;

internal sealed class GetJournalEntriesQueryHandler(
    IJournalEntryRepository repository,
    IEncryptionService encryption)
    : IRequestHandler<GetJournalEntriesQuery, Result<PagedResult<JournalEntryDto>>>
{
    public async Task<Result<PagedResult<JournalEntryDto>>> Handle(
        GetJournalEntriesQuery request, CancellationToken cancellationToken)
    {
        var paged = await repository.GetByUserIdAsync(
            request.UserId, request.Page, request.PageSize,
            request.Tag, request.From, request.To, cancellationToken);

        var dtos = paged.Items.Select(e => new JournalEntryDto(
            e.Id, e.UserId, e.Title, encryption.Decrypt(e.Content),
            e.Tags, e.MoodScore, e.IsPrivate,
            e.CreatedAt, e.UpdatedAt)).ToList();

        return Result.Success(new PagedResult<JournalEntryDto>(dtos, paged.TotalCount, request.Page, request.PageSize));
    }
}
