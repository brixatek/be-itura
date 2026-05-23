using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Journal.Application.Features.Journal.CoachSharing;

public sealed record ShareJournalEntryCommand(Guid EntryId, Guid UserId, Guid CoachId) : IRequest<Result>;
public sealed record UnshareJournalEntryCommand(Guid EntryId, Guid UserId, Guid CoachId) : IRequest<Result>;
