using Itura.Gamification.Application.DTOs;
using Itura.SharedKernel.Results;
using MediatR;

namespace Itura.Gamification.Application.Features.Gamification.Commands.AwardPoints;

public sealed record AwardPointsCommand(
    Guid UserId,
    int Points,
    string Reason,
    Guid? ReferenceId = null,
    string? ReferenceType = null) : IRequest<Result<GamificationProfileDto>>;
