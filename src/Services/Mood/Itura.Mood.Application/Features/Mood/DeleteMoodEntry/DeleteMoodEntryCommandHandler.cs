using Itura.Mood.Application.Common.Interfaces;
using Itura.Mood.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Mood.Application.Features.Mood.DeleteMoodEntry;

internal sealed class DeleteMoodEntryCommandHandler(
    IMoodEntryRepository repository,
    IMoodUnitOfWork unitOfWork)
    : IRequestHandler<DeleteMoodEntryCommand, Result>
{
    public async Task<Result> Handle(DeleteMoodEntryCommand request, CancellationToken cancellationToken)
    {
        var entry = await repository.GetByIdAsync(request.EntryId, cancellationToken);

        if (entry is null || entry.UserId != request.UserId || entry.IsDeleted)
            return Result.Failure(new Error("MoodEntry.NotFound", "Mood entry not found."));

        entry.Delete();
        repository.Update(entry);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
