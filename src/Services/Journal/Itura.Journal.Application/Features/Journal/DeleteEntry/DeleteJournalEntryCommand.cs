using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Journal.Application.Features.Journal.DeleteEntry;

public sealed record DeleteJournalEntryCommand(Guid EntryId, Guid UserId) : IRequest<Result>;
