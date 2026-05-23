using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Journal.Application.Features.Journal.CreateEntry;

public sealed record CreateJournalEntryCommand(
    Guid UserId,
    string Title,
    string Content,
    IReadOnlyList<string> Tags,
    int? MoodScore,
    bool IsPrivate) : IRequest<Result<Guid>>;
