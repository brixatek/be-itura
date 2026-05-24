using Itura.Journal.Application.Common.Interfaces;
using Itura.Journal.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Journal.Application.Features.Journal.CreateEntry;

internal sealed class CreateJournalEntryCommandHandler(
    IJournalEntryRepository repository,
    IJournalUnitOfWork unitOfWork,
    IEncryptionService encryption)
    : IRequestHandler<CreateJournalEntryCommand, Result<Guid>>
{
    private const int FreeTierLimit = 50;

    public async Task<Result<Guid>> Handle(CreateJournalEntryCommand request, CancellationToken cancellationToken)
    {
        if (!request.IsPremium)
        {
            var count = await repository.CountByUserIdAsync(request.UserId, cancellationToken);
            if (count >= FreeTierLimit)
                return Result.Failure<Guid>(Error.Validation("Journal.FreeTierLimitReached",
                    $"Free accounts are limited to {FreeTierLimit} journal entries. Upgrade to premium for unlimited journaling."));
        }

        var encryptedContent = encryption.Encrypt(request.Content);

        var result = Domain.Entities.JournalEntry.Create(
            request.UserId, request.Title, encryptedContent,
            request.Tags, request.MoodScore, request.IsPrivate);

        if (result.IsFailure) return Result.Failure<Guid>(result.Error);

        await repository.AddAsync(result.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(result.Value.Id);
    }
}
