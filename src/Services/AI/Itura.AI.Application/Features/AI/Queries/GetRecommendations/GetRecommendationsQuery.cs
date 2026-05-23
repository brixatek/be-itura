using Itura.AI.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.AI.Application.Features.AI.Queries.GetRecommendations;

public sealed record GetRecommendationsQuery(
    Guid UserId,
    string? RecommendationType,
    int Limit) : IRequest<Result<List<RecommendationDto>>>;
