using Itura.AI.Application.Common.Interfaces;
using Itura.AI.Application.DTOs;
using Itura.AI.Domain.Entities;
using Itura.AI.Domain.Repositories;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.AI.Application.Features.AI.Commands.GenerateRecommendations;

internal sealed class GenerateRecommendationsCommandHandler(
    IUserRecommendationRepository repository,
    IAIUnitOfWork unitOfWork)
    : IRequestHandler<GenerateRecommendationsCommand, Result<List<RecommendationDto>>>
{
    public async Task<Result<List<RecommendationDto>>> Handle(GenerateRecommendationsCommand request, CancellationToken cancellationToken)
    {
        await repository.DeactivateUserRecommendationsAsync(request.UserId, request.RecommendationType, cancellationToken);

        var recommendations = request.Inputs
            .Select(i => UserRecommendation.Create(request.UserId, request.RecommendationType, i.EntityType, i.EntityId, i.Score, i.Reason))
            .ToList();

        foreach (var rec in recommendations)
            await repository.AddAsync(rec, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return recommendations.Select(ToDto).ToList();
    }

    internal static RecommendationDto ToDto(UserRecommendation r) =>
        new(r.Id, r.UserId, r.RecommendationType, r.EntityType, r.EntityId, r.Score, r.Reason, r.ExpiresAt);
}
