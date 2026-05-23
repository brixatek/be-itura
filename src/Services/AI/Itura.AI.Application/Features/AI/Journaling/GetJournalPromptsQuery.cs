using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.AI.Application.Features.AI.Journaling;

public sealed record GetJournalPromptsQuery(Guid UserId) : IRequest<Result<List<string>>>;
