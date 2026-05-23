using Itura.AI.Application.DTOs;
using Itura.AI.Application.Features.AI.Commands.GenerateRecommendations;
using Itura.AI.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.AI.Application.Features.AI.Queries.GetRecommendations;

internal sealed class GetRecommendationsQueryHandler(IUserRecommendationRepository repository)
    : IRequestHandler<GetRecommendationsQuery, Result<List<RecommendationDto>>>
{
    public async Task<Result<List<RecommendationDto>>> Handle(GetRecommendationsQuery request, CancellationToken cancellationToken)
    {
        var limit = Math.Min(Math.Max(request.Limit, 1), 50);
        var recs = await repository.GetActiveByUserAsync(request.UserId, request.RecommendationType, limit, cancellationToken);
        return recs.Select(GenerateRecommendationsCommandHandler.ToDto).ToList();
    }
}
