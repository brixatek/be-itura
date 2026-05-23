using Itura.Journal.Application.Common.Interfaces;
using Itura.Journal.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Journal.Application.Features.Journal.UpdateEntry;

internal sealed class UpdateJournalEntryCommandHandler(
    IJournalEntryRepository repository,
    IJournalUnitOfWork unitOfWork)
    : IRequestHandler<UpdateJournalEntryCommand, Result>
{
    public async Task<Result> Handle(UpdateJournalEntryCommand request, CancellationToken cancellationToken)
    {
        var entry = await repository.GetByIdAsync(request.EntryId, cancellationToken);
        if (entry is null)
            return Result.Failure(Error.NotFound("JournalEntry", request.EntryId));

        if (entry.UserId != request.UserId)
            return Result.Failure(Error.Forbidden());

        var result = entry.Update(request.Title, request.Content, request.Tags, request.MoodScore, request.IsPrivate);
        if (result.IsFailure) return result;

        repository.Update(entry);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
