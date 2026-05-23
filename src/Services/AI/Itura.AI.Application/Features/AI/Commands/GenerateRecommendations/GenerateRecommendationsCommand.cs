using Itura.AI.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.AI.Application.Features.AI.Commands.GenerateRecommendations;

public sealed record GenerateRecommendationsCommand(
    Guid UserId,
    string RecommendationType,
    List<RecommendationInput> Inputs) : IRequest<Result<List<RecommendationDto>>>;

public sealed record RecommendationInput(string EntityType, Guid EntityId, float Score, string? Reason);
