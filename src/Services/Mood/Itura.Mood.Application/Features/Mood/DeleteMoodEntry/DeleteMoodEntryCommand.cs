using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Mood.Application.Features.Mood.DeleteMoodEntry;

public sealed record DeleteMoodEntryCommand(Guid EntryId, Guid UserId) : IRequest<Result>;
