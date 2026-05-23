using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Journal.Application.Features.Journal.UpdateEntry;

public sealed record UpdateJournalEntryCommand(
    Guid EntryId,
    Guid UserId,
    string Title,
    string Content,
    IReadOnlyList<string> Tags,
    int? MoodScore,
    bool IsPrivate) : IRequest<Result>;
