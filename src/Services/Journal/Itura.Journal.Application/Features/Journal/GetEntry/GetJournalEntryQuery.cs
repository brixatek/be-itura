using Itura.Journal.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Journal.Application.Features.Journal.GetEntry;

public sealed record GetJournalEntryQuery(Guid EntryId, Guid UserId) : IRequest<Result<JournalEntryDto>>;
