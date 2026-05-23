using Itura.Journal.Application.Common.Interfaces;
using Itura.Journal.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Journal.Application.Features.Journal.DeleteEntry;

internal sealed class DeleteJournalEntryCommandHandler(
    IJournalEntryRepository repository,
    IJournalUnitOfWork unitOfWork)
    : IRequestHandler<DeleteJournalEntryCommand, Result>
{
    public async Task<Result> Handle(DeleteJournalEntryCommand request, CancellationToken cancellationToken)
    {
        var entry = await repository.GetByIdAsync(request.EntryId, cancellationToken);
        if (entry is null)
            return Result.Failure(Error.NotFound("JournalEntry", request.EntryId));

        if (entry.UserId != request.UserId)
            return Result.Failure(Error.Forbidden());

        entry.Delete();
        repository.Update(entry);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
