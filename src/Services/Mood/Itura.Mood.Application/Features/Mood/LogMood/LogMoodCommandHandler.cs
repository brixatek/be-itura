using Itura.Mood.Application.Common.Interfaces;
using Itura.Mood.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Mood.Application.Features.Mood.LogMood;

internal sealed class LogMoodCommandHandler(
    IMoodEntryRepository repository,
    IMoodUnitOfWork unitOfWork)
    : IRequestHandler<LogMoodCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(LogMoodCommand request, CancellationToken cancellationToken)
    {
        var result = Domain.Entities.MoodEntry.Create(
            request.UserId, request.Score, request.Note, request.Emotions, request.LoggedAt);

        if (result.IsFailure) return Result.Failure<Guid>(result.Error);

        await repository.AddAsync(result.Value, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(result.Value.Id);
    }
}
