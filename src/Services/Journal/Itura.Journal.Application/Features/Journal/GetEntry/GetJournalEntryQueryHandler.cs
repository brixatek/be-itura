using Itura.Journal.Application.Common.Interfaces;
using Itura.Journal.Application.DTOs;
using Itura.Journal.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Journal.Application.Features.Journal.GetEntry;

internal sealed class GetJournalEntryQueryHandler(
    IJournalEntryRepository repository,
    IEncryptionService encryption)
    : IRequestHandler<GetJournalEntryQuery, Result<JournalEntryDto>>
{
    public async Task<Result<JournalEntryDto>> Handle(GetJournalEntryQuery request, CancellationToken cancellationToken)
    {
        var entry = await repository.GetByIdAsync(request.EntryId, cancellationToken);
        if (entry is null)
            return Result.Failure<JournalEntryDto>(Error.NotFound("JournalEntry", request.EntryId));

        if (entry.UserId != request.UserId)
            return Result.Failure<JournalEntryDto>(Error.Forbidden());

        return Result.Success(new JournalEntryDto(
            entry.Id, entry.UserId, entry.Title, encryption.Decrypt(entry.Content),
            entry.Tags, entry.MoodScore, entry.IsPrivate,
            entry.CreatedAt, entry.UpdatedAt));
    }
}
