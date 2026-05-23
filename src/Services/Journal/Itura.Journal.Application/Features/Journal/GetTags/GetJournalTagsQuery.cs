using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Journal.Application.Features.Journal.GetTags;

public sealed record GetJournalTagsQuery(Guid UserId) : IRequest<Result<IReadOnlyList<string>>>;
