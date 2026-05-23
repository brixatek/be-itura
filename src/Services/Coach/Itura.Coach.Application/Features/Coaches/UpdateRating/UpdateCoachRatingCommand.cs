using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Coach.Application.Features.Coaches.UpdateRating;

public sealed record UpdateCoachRatingCommand(
    Guid CoachId,
    double NewAverageRating,
    int TotalReviews) : IRequest<Result>;
