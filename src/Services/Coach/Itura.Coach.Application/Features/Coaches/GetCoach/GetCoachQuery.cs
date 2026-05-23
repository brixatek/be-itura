using Itura.Coach.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Coach.Application.Features.Coaches.GetCoach;

public sealed record GetCoachQuery(Guid CoachId) : IRequest<Result<CoachDto>>;
