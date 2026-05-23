using Itura.Journal.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Journal.Application.Features.Journal.GetTags;

internal sealed class GetJournalTagsQueryHandler(IJournalEntryRepository repository)
    : IRequestHandler<GetJournalTagsQuery, Result<IReadOnlyList<string>>>
{
    public async Task<Result<IReadOnlyList<string>>> Handle(
        GetJournalTagsQuery request, CancellationToken cancellationToken)
    {
        var tags = await repository.GetTagsByUserIdAsync(request.UserId, cancellationToken);
        return Result.Success(tags);
    }
}
