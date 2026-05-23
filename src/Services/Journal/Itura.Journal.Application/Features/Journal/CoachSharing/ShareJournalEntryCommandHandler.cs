using Itura.Journal.Application.Common.Interfaces;
using Itura.Journal.Domain.Entities;
using Itura.Journal.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Journal.Application.Features.Journal.CoachSharing;

internal sealed class ShareJournalEntryCommandHandler(
    IJournalEntryRepository entryRepository,
    IJournalCoachShareRepository shareRepository,
    IJournalUnitOfWork unitOfWork)
    : IRequestHandler<ShareJournalEntryCommand, Result>,
      IRequestHandler<UnshareJournalEntryCommand, Result>
{
    public async Task<Result> Handle(ShareJournalEntryCommand request, CancellationToken cancellationToken)
    {
        var entry = await entryRepository.GetByIdAsync(request.EntryId, cancellationToken);
        if (entry is null) return Result.Failure(Error.NotFound("JournalEntry", request.EntryId));
        if (entry.UserId != request.UserId) return Result.Failure(Error.Forbidden());

        var existing = await shareRepository.GetAsync(request.EntryId, request.CoachId, cancellationToken);
        if (existing is not null && !existing.IsRevoked)
            return Result.Success(); // Already shared — idempotent

        if (existing is not null && existing.IsRevoked)
        {
            // Re-share by creating a new share record
        }

        var share = JournalCoachShare.Create(request.EntryId, request.CoachId);
        await shareRepository.AddAsync(share, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> Handle(UnshareJournalEntryCommand request, CancellationToken cancellationToken)
    {
        var entry = await entryRepository.GetByIdAsync(request.EntryId, cancellationToken);
        if (entry is null) return Result.Failure(Error.NotFound("JournalEntry", request.EntryId));
        if (entry.UserId != request.UserId) return Result.Failure(Error.Forbidden());

        var share = await shareRepository.GetAsync(request.EntryId, request.CoachId, cancellationToken);
        if (share is null || share.IsRevoked) return Result.Success(); // Already unshared — idempotent

        share.Revoke();
        shareRepository.Update(share);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
