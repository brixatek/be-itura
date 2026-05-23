using Itura.Journal.Application.Common.Interfaces;
using Itura.Journal.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Journal.Application.Features.Journal.CreateEntry;

internal sealed class CreateJournalEntryCommandHandler(
    IJournalEntryRepository repository,
    IJournalUnitOfWork unitOfWork)
    : IRequestHandler<CreateJournalEntryCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateJournalEntryCommand request, CancellationToken cancellationToken)
    {
        var result = Domain.Entities.JournalEntry.Create(
            request.UserId, request.Title, request.Content,
            request.Tags, request.MoodScore, request.IsPrivate);

        if (result.IsFailure) return Result.Failure<Guid>(result.Error);

        await repository.AddAsync(result.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(result.Value.Id);
    }
}
